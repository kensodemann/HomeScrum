using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate.Linq;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.MockingKernel.Moq;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class ProjectsControllerTest
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;
      private static Mock<IRepository<ProjectStatus>> _projectStatusRepository;  // Should go away once the mapper is fixed

      private Mock<ILogger> _logger;

      private User _currentUser;
      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
         
         CreateMockIOCKernel();
         CreateStaticRepositories();
         IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         Database.Build();
         Users.Load();
         ProjectStatuses.Load();
         Projects.Load();

         SetupProjectStatusRepository();
         SetupCurrentUser();
         SetupLogger();
      }
      #endregion

      [TestMethod]
      public void Index_ReturnsViewWithAllItems()
      {
         var controller = CreateController();

         var view = controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<DomainObjectViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( Projects.ModelData.Count(), model.Count() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var controller = CreateController();
         var model = Projects.ModelData[2];

         var view = controller.Details( model.Id ) as ViewResult;

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
         var controller = CreateController();
         var id = Guid.NewGuid();

         var result = controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as ProjectEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesProjectStatusList_NothingSelected()
      {
         var controller = CreateController();

         var expected = ProjectStatuses.ModelData
            .Where( x => x.StatusCd == 'A' )
            .OrderBy( x => x.SortSequence );

         var result = controller.Create() as ViewResult;
         var viewModel = result.Model as ProjectEditorViewModel;

         AssertSelectListOrderAndContents( expected, viewModel.Statuses );
      }

      [TestMethod]
      public void CreatePost_SavesModelIfNewViewModelIsValid()
      {
         var controller = CreateController();
         var model = CreateProjectEditorViewModel();

         var result = controller.Create( model, _principal.Object );

         using (var session = Database.GetSession())
         {
            var items = session.Query<Project>()
               .Where( x => x.Name == model.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( model.Name, items[0].Name );
            Assert.AreEqual( model.Description, items[0].Description );
         }
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateController();
         var model = CreateProjectEditorViewModel();

         var result = controller.Create( model, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotSaveModelIfModelIsNotValid()
      {
         var controller = CreateController();
         var model = CreateProjectEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, _principal.Object );

         using (var session = Database.GetSession())
         {
            var items = session.Query<Project>()
               .Where( x => x.Name == model.Name )
               .ToList();
         }
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var controller = CreateController();
         var model = CreateProjectEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( model, result.Model );
      }

      [TestMethod]
      public void CreatePost_InitializesProjectStatusList_ActiveItemSelected()
      {
         var controller = CreateController();
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
      public void CreatePost_CopiesMessagesToModelStateIfDatabaseValidationFails()
      {
         var controller = CreateController();
         var viewModel = CreateProjectEditorViewModel();

         viewModel.Name = "";
         var result = controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 1, controller.ModelState.Count );
         Assert.IsTrue( controller.ModelState.ContainsKey( "Name" ) );
         Assert.IsTrue( result is ViewResult );
      }


      [TestMethod]
      public void CreatePost_SetsLastModifiedUserIdToCurrentUser()
      {
         var controller = CreateController();
         var viewModel = CreateProjectEditorViewModel();

         var user = Users.ModelData[0];
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         controller.Create( viewModel, _principal.Object );

         _userIdentity.Verify();

         using (var session = Database.GetSession())
         {
            var items = session.Query<Project>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
         }
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var controller = CreateController();
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
         var controller = CreateController();
         var model = Projects.ModelData[0];

         var expected = ProjectStatuses.ModelData
            .Where( x => x.StatusCd == 'A' )
            .OrderBy( x => x.SortSequence );

         var result = controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as ProjectEditorViewModel;

         AssertSelectListOrderAndContents( expected, viewModel.Statuses, model.Status.Id );
      }

      [TestMethod]
      public void EditGet_InitializesProjectStatuses_IncludesInactiveStatusIfSelected()
      {
         var controller = CreateController();
         var model = Projects.ModelData
            .First( x => x.Status.StatusCd == 'I' );

         var expected = ProjectStatuses.ModelData
            .Where( x => x.StatusCd == 'A' || x.Id == model.Status.Id )
            .OrderBy( x => x.SortSequence );

         var result = controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as ProjectEditorViewModel;

         AssertSelectListOrderAndContents( expected, viewModel.Statuses, model.Status.Id );
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      {
         var controller = CreateController();
         
         var result = controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_UpdatesDatabaseIfModelValid()
      {
         var controller = CreateController();
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         viewModel.Name += " Modified";
         controller.Edit( viewModel, _principal.Object );

         using (var session = Database.GetSession())
         {
            var item = session.Get<Project>( viewModel.Id );
            Assert.AreEqual( viewModel.Name, item.Name );
         }
      }

      [TestMethod]
      public void EditPost_DoesNotUpdateDatabaseIfModelIsNotValid()
      {
         var controller = CreateController();
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var origName = viewModel.Name;
         viewModel.Name += " Modified";
         controller.Edit( viewModel, _principal.Object );

         using (var session = Database.GetSession())
         {
            var item = session.Get<Project>( viewModel.Id );
            Assert.AreNotEqual( viewModel.Name, item.Name );
            Assert.AreEqual( origName, item.Name );
         }
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateController();
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
         var controller = CreateController();
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
      public void EditPost_CopiesMessagesToModelStateIfUpdateFails()
      {
         var controller = CreateController();
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         viewModel.Name = "";
         var result = controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 1, controller.ModelState.Count );
         Assert.IsTrue( controller.ModelState.ContainsKey( "Name" ) );
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_SetsLastModifiedUserId()
      {
         var controller = CreateController();
         var model = Projects.ModelData[3];
         var viewModel = CreateProjectEditorViewModel( model );

         var user = Users.ModelData.First( x => x.Id != viewModel.LastModifiedUserId );
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         controller.Edit( viewModel, _principal.Object );

         _userIdentity.Verify();

         using (var session = Database.GetSession())
         {
            var items = session.Query<Project>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
         }
      }

      [TestMethod]
      public void EditGet_ReInitializesProjectStatusesIfModelInvalid_ProjectStatusSelected()
      {
         var controller = CreateController();
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
      private static void AssertSelectListOrderAndContents( IEnumerable<ProjectStatus> expected, IEnumerable<SelectListItem> actual, Guid selectedId = default( Guid ) )
      {
         var expectedCount = expected.Count();

         Assert.AreEqual( expectedCount, actual.Count() );
         for (var i = 0; i < expectedCount; i++)
         {
            var currentActualStatus = actual.ElementAt( i );
            var currentActualStatusId = new Guid( currentActualStatus.Value );

            Assert.AreEqual( expected.ElementAt( i ).Id, currentActualStatusId );
            Assert.AreEqual( expected.ElementAt( i ).Name, currentActualStatus.Text );
            if (currentActualStatusId == selectedId)
            {
               Assert.IsTrue( currentActualStatus.Selected, "Status should be selected" );
            }
            else
            {
               Assert.IsFalse( currentActualStatus.Selected, "Status should not be selected" );
            }
         }
      }

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

      private static void SetupProjectStatusRepository()
      {
         _projectStatusRepository.Setup( x => x.GetAll() ).Returns( ProjectStatuses.ModelData );
         foreach (var model in ProjectStatuses.ModelData)
         {
            _projectStatusRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }
      }

      private void SetupCurrentUser()
      {
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         _currentUser = new User()
         {
            Id = Guid.NewGuid(),
            UserName = "test",
            FirstName = "Fred"
         };
      }

      private ProjectsController CreateController()
      {
         var controller = new ProjectsController( new PropertyNameTranslator<Project, ProjectEditorViewModel>(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }

      private void SetupLogger()
      {
         _logger = new Mock<ILogger>();
      }

      private static void CreateStaticRepositories()
      {
         _projectStatusRepository = _iocKernel.GetMock<IRepository<ProjectStatus>>();
      }
      #endregion
   }
}
