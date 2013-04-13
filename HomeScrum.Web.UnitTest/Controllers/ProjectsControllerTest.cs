using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class ProjectsControllerTest
   {
      private static Mock<IRepository<ProjectStatus>> _projectStatusRepository;
      private static MoqMockingKernel _iocKernel;

      private Mock<IValidator<Project>> _validator;
      private Mock<IRepository<Project>> _projectRepository;
      private ProjectsController _controller;

      private User _currentUser;
      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;
      private static Mock<IUserRepository> _userRepository;


      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         CreateMockIOCKernel();
         InitializeTestData();
         CreateStaticRepositories();
         IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         SetupCurrentUser();
         SetupValidator();
         SetupProjectRepository();

         CreateController();
      }

      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _projectRepository.Setup( x => x.GetAll() )
            .Returns( Projects.ModelData );

         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( IEnumerable<ProjectViewModel> ) );
      }

      [TestMethod]
      public void Index_GetsAllItems()
      {
         _controller.Index();

         _projectRepository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = Projects.ModelData[2];

         _projectRepository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var view = _controller.Details( model.Id ) as ViewResult;

         _projectRepository.Verify( x => x.Get( model.Id ), Times.Once() );

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( ProjectViewModel ) );
         Assert.AreEqual( model.Id, ((ProjectViewModel)view.Model).Id );
         Assert.AreEqual( model.Name, ((ProjectViewModel)view.Model).Name );
         Assert.AreEqual( model.Description, ((ProjectViewModel)view.Model).Description );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         _projectRepository.Setup( x => x.Get( id ) ).Returns( null as Project );

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as ProjectEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesProjectStatusList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as ProjectEditorViewModel;

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.ProjectStatuses.Count() );
         foreach (var item in model.ProjectStatuses)
         {
            var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_CallsRepositoryAddIfNewModelIsValid()
      {
         var model = CreateProjectEditorViewModel();

         var result = _controller.Create( model, _principal.Object );

         _projectRepository.Verify( x => x.Add( It.Is<Project>( p => p.Id == model.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var model = CreateProjectEditorViewModel();

         var result = _controller.Create( model, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotCallRepositoryAddIfModelIsNotValid()
      {
         var model = CreateProjectEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model, _principal.Object );

         _projectRepository.Verify( x => x.Add( It.IsAny<Project>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var model = CreateProjectEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( model, result.Model );
      }

      [TestMethod]
      public void CreatePost_InitializesProjectStatusList_NothingSelected()
      {
         var model = CreateProjectEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as ProjectEditorViewModel;

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), returnedModel.ProjectStatuses.Count() );
         foreach (var item in returnedModel.ProjectStatuses)
         {
            var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_PassesModelToValidator()
      {
         var viewModel = CreateProjectEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _validator.Verify( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), TransactionType.Insert ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var viewModel = CreateProjectEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = _controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( messages.Count, _controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( _controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var messages = CreateStockErrorMessages();
         var viewModel = CreateProjectEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void CreatePost_SetsLastModifiedUserIdToCurrentUser()
      {
         var viewModel = CreateProjectEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _userIdentity.Verify();
         _userRepository.Verify(); 
         _projectRepository.Verify( x => x.Add( It.Is<Project>( p => p.Id == viewModel.Id && p.LastModifiedUserRid == _currentUser.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_CallsRepositoryGet()
      {
         Guid id = Guid.NewGuid();
         _controller.Edit( id );

         _projectRepository.Verify( x => x.Get( id ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var model = Projects.ModelData[3];
         _projectRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         var returnedModel = result.Model as ProjectEditorViewModel;
         Assert.IsNotNull( returnedModel );
         Assert.AreEqual( model.Id, returnedModel.Id );
      }

      [TestMethod]
      public void EditGet_InitializesProjectStatuses_ProjectStatusSelected()
      {
         var model = Projects.ModelData[0];
         _projectRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as ProjectEditorViewModel;

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.ProjectStatuses.Count() );
         foreach (var item in viewModel.ProjectStatuses)
         {
            var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.ProjectStatus.Id.ToString() != item.Value && !item.Selected) ||
                           (model.ProjectStatus.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      {
         _projectRepository.Setup( x => x.Get( It.IsAny<Guid>() ) ).Returns( null as Project );

         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_CallRepositoryUpdateIfModelValid()
      {
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         _controller.Edit( viewModel, _principal.Object );

         _projectRepository.Verify( x => x.Update( It.Is<Project>( p => p.Id == model.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      {
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         _controller.Edit( viewModel, _principal.Object );

         _projectRepository.Verify( x => x.Update( It.IsAny<Project>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         var result = _controller.Edit( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsViewIfModelIsNotValid()
      {
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( ProjectEditorViewModel ) );
         Assert.AreEqual( model.Id, ((ProjectEditorViewModel)result.Model).Id );
         Assert.AreEqual( model.Name, ((ProjectEditorViewModel)result.Model).Name );
         Assert.AreEqual( model.Description, ((ProjectEditorViewModel)result.Model).Description );
      }

      [TestMethod]
      public void EditPost_PassesModelToValidator()
      {
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         _controller.Edit( viewModel, _principal.Object );

         _validator.Verify( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), TransactionType.Update ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( messages.Count, _controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( _controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var messages = CreateStockErrorMessages();
         var model = Projects.ModelData[3];
         var viewModel = new ProjectEditorViewModel()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            LastModifiedUserId = model.LastModifiedUserRid,
            ProjectStatusId = model.ProjectStatus.Id,
            ProjectStatusName = model.ProjectStatus.Name
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditPost_SetsLastModifiedUserId()
      {
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         _controller.Edit( viewModel, _principal.Object );

         _userIdentity.Verify();
         _userRepository.Verify();
         _projectRepository.Verify( x => x.Update( It.Is<Project>( p => p.Id == model.Id && p.LastModifiedUserRid == _currentUser.Id ) ), Times.Once() );
      }

      #region private helpers
      private static void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
      }

      private static void IntializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      private static void CreateStaticRepositories()
      {
         _projectStatusRepository = _iocKernel.GetMock<IRepository<ProjectStatus>>();
         _projectStatusRepository.Setup( x => x.GetAll() ).Returns( ProjectStatuses.ModelData );
         foreach (var model in ProjectStatuses.ModelData)
         {
            _projectStatusRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }
      }

      private static void InitializeTestData()
      {
         Users.CreateTestModelData( initializeIds: true );
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         Projects.CreateTestModelData( initializeIds: true );
      }

      ICollection<KeyValuePair<string, string>> CreateStockErrorMessages()
      {
         var messages = new List<KeyValuePair<string, string>>();

         messages.Add( new KeyValuePair<string, string>( "Name", "Name is not unique" ) );
         messages.Add( new KeyValuePair<string, string>( "SomethingElse", "Another Message" ) );

         return messages;
      }

      private ProjectEditorViewModel CreateProjectEditorViewModel()
      {
         return new ProjectEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "New Project",
            Description = "This is a test",
            LastModifiedUserId = default( Guid ),
            ProjectStatusId = ProjectStatuses.ModelData[0].Id,
            ProjectStatusName = ProjectStatuses.ModelData[0].Name
         };
      }

      private ProjectEditorViewModel CreateProjectEditorViewModel( Project project )
      {
         return new ProjectEditorViewModel()
         {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            LastModifiedUserId = project.LastModifiedUserRid,
            ProjectStatusId = project.ProjectStatus.Id,
            ProjectStatusName = project.ProjectStatus.Name
         };
      }

      private void SetupCurrentUser()
      {
         _userRepository = new Mock<IUserRepository>();
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         _currentUser = new User()
         {
            Id = Guid.NewGuid(),
            UserName = "test",
            FirstName = "Fred"
         };
         _userRepository
            .Setup( x => x.Get( "test" ) )
            .Returns( _currentUser );
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( "test" );
      }

      private void CreateController()
      {
         _controller = new ProjectsController( _projectRepository.Object, _projectStatusRepository.Object, _userRepository.Object, _validator.Object );
         _controller.ControllerContext = new ControllerContext();
      }

      private void SetupProjectRepository()
      {
         _projectRepository = new Mock<IRepository<Project>>();
         _projectRepository.Setup( x => x.GetAll() ).Returns( Projects.ModelData );
      }

      private void SetupValidator()
      {
         _validator = new Mock<IValidator<Project>>();
         _validator.Setup( x => x.ModelIsValid( It.IsAny<Project>(), It.IsAny<TransactionType>() ) ).Returns( true );
      }
      #endregion
   }
}
