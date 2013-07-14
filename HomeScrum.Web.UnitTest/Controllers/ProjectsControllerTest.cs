using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Ninject;
using Ninject.Extensions.Logging;
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
      #region Test Setup
      private static MoqMockingKernel _iocKernel;
      private static Mock<IRepository<ProjectStatus>> _projectStatusRepository;  // Should go away once the mapper is fixed

      private Mock<IValidator<Project>> _validator;
      private Mock<ILogger> _logger;

      private User _currentUser;
      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

      private Mock<ISessionFactory> _sessionFactory;
      private Mock<ISession> _session;
      private Mock<ITransaction> _transaction;


      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
         Database.Build();
         Users.Load();
         ProjectStatuses.Load();
         Projects.Load();

         CreateMockIOCKernel();
         CreateStaticRepositories();
         IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         //Database.Build();
         //Users.Load();
         //ProjectStatuses.Load();
         //Projects.Load();

         SetupSessionFactory();
         SetupCurrentUser();
         SetupValidator();
         SetupLogger();
      }
      #endregion

      [TestMethod]
      public void Index_ReturnsViewWithAllItems()
      {
         var controller = CreateDatabaseConnectedController();

         var view = controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<DomainObjectViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( Projects.ModelData.Count(), model.Count() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var controller = CreateDatabaseMockedController();
         var model = Projects.ModelData[2];

         _session.Setup( x => x.Get<Project>( model.Id ) )
            .Returns( model );

         var view = controller.Details( model.Id ) as ViewResult;

         _session.Verify( x => x.Get<Project>( model.Id ), Times.Once() );

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
         var controller = CreateDatabaseMockedController();
         var id = Guid.NewGuid();

         _session.Setup( x => x.Get<Project>( id ) ).Returns( null as Project );

         var result = controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var controller = CreateDatabaseConnectedController();

         var result = controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as ProjectEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesProjectStatusList_NothingSelected()
      {
         var controller = CreateDatabaseConnectedController();

         var result = controller.Create() as ViewResult;

         var model = result.Model as ProjectEditorViewModel;

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.Statuses.Count() );
         foreach (var item in model.Statuses)
         {
            var status = ProjectStatuses.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_CallsSaveAndCommitIfNewViewModelIsValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = CreateProjectEditorViewModel();

         var result = controller.Create( model, _principal.Object );

         _session.Verify( x => x.BeginTransaction(), Times.Once() );
         _transaction.Verify( x => x.Commit(), Times.Once() );
         _session.Verify( x => x.Save( It.Is<Project>( p => p.Id == model.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = CreateProjectEditorViewModel();

         var result = controller.Create( model, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotCallSaveOrCommitAddIfModelIsNotValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = CreateProjectEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, _principal.Object );

         _session.Verify( x => x.BeginTransaction(), Times.Never() );
         _transaction.Verify( x => x.Commit(), Times.Never() );
         _session.Verify( x => x.Save( It.IsAny<Project>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = CreateProjectEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( model, result.Model );
      }

      [TestMethod]
      public void CreatePost_InitializesProjectStatusList_ActiveItemSelected()
      {
         var controller = CreateDatabaseConnectedController();
         var model = CreateProjectEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as ProjectEditorViewModel;

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), returnedModel.Statuses.Count() );
         foreach (var item in returnedModel.Statuses)
         {
            var statusId = new Guid( item.Value );
            var status = ProjectStatuses.ModelData.First( x => x.Id == statusId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (statusId == model.StatusId) ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_PassesModelToValidator()
      {
         var controller = CreateDatabaseMockedController();
         var viewModel = CreateProjectEditorViewModel();

         controller.Create( viewModel, _principal.Object );

         _validator.Verify( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), TransactionType.Insert ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var viewModel = CreateProjectEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( messages.Count, controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var viewModel = CreateProjectEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 0, controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void CreatePost_SetsLastModifiedUserIdToCurrentUser()
      {
         var controller = CreateDatabaseMockedController();
         var viewModel = CreateProjectEditorViewModel();

         controller.Create( viewModel, _principal.Object );

         _userIdentity.Verify();
         _session.Verify( x => x.BeginTransaction(), Times.Once() );
         _transaction.Verify( x => x.Commit(), Times.Once() );
         _session.Verify( x => x.Save( It.Is<Project>( p => p.Id == viewModel.Id && p.LastModifiedUserRid == _currentUser.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_CallsSessionGet()
      {
         var controller = CreateDatabaseMockedController();

         Guid id = Guid.NewGuid();
         controller.Edit( id );

         _session.Verify( x => x.Get<Project>( id ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var controller = CreateDatabaseConnectedController();
         var model = Projects.ModelData[3];

         var result = controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         var returnedModel = result.Model as ProjectEditorViewModel;
         Assert.IsNotNull( returnedModel );
         Assert.AreEqual( model.Id, returnedModel.Id );
      }

      [TestMethod]
      public void EditGet_InitializesProjectStatuses_ProjectStatusSelected()
      {
         var controller = CreateDatabaseConnectedController();
         var model = Projects.ModelData[0];

         var result = controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as ProjectEditorViewModel;

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var statusId = new Guid( item.Value );
            var status = ProjectStatuses.ModelData.First( x => x.Id == statusId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id != statusId && !item.Selected) ||
                           (model.Status.Id == statusId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      {
         var controller = CreateDatabaseMockedController();
         _session.Setup( x => x.Get<Project>( It.IsAny<Guid>() ) ).Returns( null as Project );

         var result = controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_CallsUpdateIfModelValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         controller.Edit( viewModel, _principal.Object );

         _session.Verify( x => x.BeginTransaction(), Times.Once() );
         _transaction.Verify( x => x.Commit(), Times.Once() );
         _session.Verify( x => x.Update( It.Is<Project>( p => p.Id == model.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallUpdateIfModelIsNotValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         controller.Edit( viewModel, _principal.Object );

         _session.Verify( x => x.BeginTransaction(), Times.Never() );
         _transaction.Verify( x => x.Commit(), Times.Never() );
         _session.Verify( x => x.Update( It.IsAny<Project>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         var result = controller.Edit( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsViewIfModelIsNotValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( ProjectEditorViewModel ) );
         Assert.AreEqual( model.Id, ((ProjectEditorViewModel)result.Model).Id );
         Assert.AreEqual( model.Name, ((ProjectEditorViewModel)result.Model).Name );
         Assert.AreEqual( model.Description, ((ProjectEditorViewModel)result.Model).Description );
      }

      [TestMethod]
      public void EditPost_PassesModelToValidator()
      {
         var controller = CreateDatabaseMockedController();
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         controller.Edit( viewModel, _principal.Object );

         _validator.Verify( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), TransactionType.Update ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( messages.Count, controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var model = Projects.ModelData[3];
         var viewModel = new ProjectEditorViewModel()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            LastModifiedUserId = model.LastModifiedUserRid,
            StatusId = model.Status.Id,
            StatusName = model.Status.Name
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<Project>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 0, controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditPost_SetsLastModifiedUserId()
      {
         var controller = CreateDatabaseMockedController();
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         controller.Edit( viewModel, _principal.Object );

         _userIdentity.Verify();
         _session.Verify( x => x.BeginTransaction(), Times.Once() );
         _transaction.Verify( x => x.Commit(), Times.Once() );
         _session.Verify( x => x.Update( It.Is<Project>( p => p.Id == model.Id && p.LastModifiedUserRid == _currentUser.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReInitializesProjectStatusesIfModelInvalid_ProjectStatusSelected()
      {
         var controller = CreateDatabaseConnectedController();
         var model = Projects.ModelData[0];
         var viewModel = CreateProjectEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var statusId = new Guid( item.Value );
            var status = ProjectStatuses.ModelData.First( x => x.Id == statusId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id != statusId && !item.Selected) ||
                           (model.Status.Id == statusId && item.Selected) );
         }
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
            StatusId = ProjectStatuses.ModelData[0].Id,
            StatusName = ProjectStatuses.ModelData[0].Name
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
            StatusId = project.Status.Id,
            StatusName = project.Status.Name
         };
      }

      private void SetupCurrentUser()
      {
         Mock<ICriteria> _userQuery = new Mock<ICriteria>();

         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         _currentUser = new User()
         {
            Id = Guid.NewGuid(),
            UserName = "test",
            FirstName = "Fred"
         };

         _session
            .Setup( x => x.CreateCriteria( typeof( User ) ) )
            .Returns( _userQuery.Object );
         _userQuery
            .Setup( x => x.Add( It.IsAny<SimpleExpression>() ) )
            .Returns( _userQuery.Object );
         _userQuery
            .Setup( x => x.UniqueResult() )
            .Returns( _currentUser );

         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( "test" );
      }

      private ProjectsController CreateDatabaseConnectedController()
      {
         var controller = new ProjectsController(
            _validator.Object, new PropertyNameTranslator<Project, ProjectEditorViewModel>(), _logger.Object, NHibernateHelper.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }

      private ProjectsController CreateDatabaseMockedController()
      {
         var controller = new ProjectsController(
            _validator.Object, new PropertyNameTranslator<Project, ProjectEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }

      private void SetupLogger()
      {
         _logger = new Mock<ILogger>();
      }

      private void SetupValidator()
      {
         _validator = new Mock<IValidator<Project>>();
         _validator.Setup( x => x.ModelIsValid( It.IsAny<Project>(), It.IsAny<TransactionType>() ) ).Returns( true );
      }

      private void SetupSessionFactory()
      {
         _sessionFactory = new Mock<ISessionFactory>();
         _session = new Mock<ISession>();
         var _query = new Mock<ICriteria>();
         _transaction = new Mock<ITransaction>();

         _sessionFactory
            .Setup( x => x.OpenSession() )
            .Returns( _session.Object );

         _session
            .Setup( x => x.BeginTransaction() )
            .Returns( _transaction.Object );

         _session
            .Setup( x => x.CreateCriteria( It.IsAny<Type>() ))
            .Returns( _query.Object );
         _query
            .Setup( x => x.Add( It.IsAny<ICriterion>() ) )
            .Returns( _query.Object );
         _query
            .Setup( x => x.List<SelectListItem>() )
            .Returns( new List<SelectListItem>() );

         _query
            .Setup( x => x.SetProjection( It.IsAny<ProjectionList>() ) )
            .Returns( _query.Object );
         _query
            .Setup( x => x.SetResultTransformer( It.IsAny<IResultTransformer>() ) )
            .Returns( _query.Object );
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
      #endregion
   }
}
