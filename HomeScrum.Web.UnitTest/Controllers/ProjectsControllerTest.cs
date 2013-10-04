using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Linq;
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

      private Mock<ILogger> _logger;

      private Mock<ControllerContext> _controllerConext;
      private Stack<NavigationData> _navigationStack;

      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();

         IntializeMapper();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         SetupSession();
         CreateMockIOCKernel();
         BuildDatabase();

         SetupCurrentUser();
         SetupLogger();
         SetupControllerContext();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
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
      public void CreateGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         controller.Create( callingAction: "Index" );
         var viewModel = ((ViewResult)controller.Create( callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         controller.Create( callingAction: "Index" );
         controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         controller.Create( callingAction: "Index" );
         controller.Create( callingAction: "Index" );

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 3, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void CreateGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         controller.Create( callingAction: "Index" );
         controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((ViewResult)controller.Create()).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }

      [TestMethod]
      public void CreatePost_SavesModelIfNewViewModelIsValid()
      {
         var controller = CreateController();
         var model = CreateProjectEditorViewModel();

         var result = controller.Create( model, _principal.Object );

         _session.Clear();
         var items = _session.Query<Project>()
            .Where( x => x.Name == model.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( model.Name, items[0].Name );
         Assert.AreEqual( model.Description, items[0].Description );
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

         _session.Clear();
         var items = _session.Query<Project>()
            .Where( x => x.Name == model.Name )
            .ToList();
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

         _session.Clear();
         var items = _session.Query<Project>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
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
      public void EditGet_ReturnsNoDataFoundIfModelNotFound()
      {
         var controller = CreateController();

         var result = controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var controller = CreateController();
         var id = Projects.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         controller.Edit( id, callingAction: "Index" );
         var viewModel = ((ViewResult)controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var controller = CreateController();
         var id = Projects.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         controller.Edit( id, callingAction: "Index" );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Edit( id, callingAction: "Index" );
         controller.Edit( id, callingAction: "Index" );

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 3, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void EditGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var controller = CreateController();
         var id = Projects.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         controller.Edit( id, callingAction: "Index" );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((ViewResult)controller.Edit( id )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }

      [TestMethod]
      public void EditPost_UpdatesDatabaseIfModelValid()
      {
         var controller = CreateController();
         var model = Projects.ModelData[2];
         var viewModel = CreateProjectEditorViewModel( model );

         viewModel.Name += " Modified";
         controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<Project>( viewModel.Id );
         Assert.AreEqual( viewModel.Name, item.Name );
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

         _session.Clear();
         var item = _session.Get<Project>( viewModel.Id );
         Assert.AreNotEqual( viewModel.Name, item.Name );
         Assert.AreEqual( origName, item.Name );
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

         _session.Clear();
         var items = _session.Query<Project>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
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


      private void BuildDatabase()
      {
         Database.Build( _session );
         Projects.Load( _sessionFactory.Object );
      }

      private void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( _sessionFactory.Object );
      }

      private static void IntializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
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
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         // In other places where we use a random user, we use the first active one.
         // Use the first inactive user here just to ensure it is a different user.
         var currentUser = Users.ModelData.First( x => x.StatusCd == 'I' );
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( currentUser.UserName );
      }

      private ProjectsController CreateController()
      {
         var controller = new ProjectsController( new PropertyNameTranslator<Project, ProjectEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }

      private void SetupLogger()
      {
         _logger = new Mock<ILogger>();
      }

      private void SetupControllerContext()
      {
         _controllerConext = new Mock<ControllerContext>();
         _controllerConext
            .SetupSet( x => x.HttpContext.Session["NavigationStack"] = It.IsAny<Stack<NavigationData>>() )
            .Callback( ( string name, object m ) => { _navigationStack = (Stack<NavigationData>)m; } );
         _controllerConext
            .Setup( x => x.HttpContext.Session["NavigationStack"] )
            .Returns( () => _navigationStack );
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }
      #endregion
   }
}
