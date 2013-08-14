﻿using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.Sprints;
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
   public class SprintsControllerTest
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private Mock<ILogger> _logger;

      private Mock<ControllerContext> _controllerConext;
      private Stack<NavigationData> _navigationStack;

      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

      private SprintsController _controller;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
         InitializeMapper();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         BuildMocks();
         CreateMockIOCKernel();
         SetupSession();
         BuildDatabase();
         SetupControllerContext();
         SetupCurrentUser();

         CreateController();
      }

      private void BuildMocks()
      {
         _sessionFactory = new Mock<ISessionFactory>();
         _logger = new Mock<ILogger>();
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      private void BuildDatabase()
      {
         Database.Build( _session );
         Sprints.Load( _sessionFactory.Object );
      }

      private void CreateController()
      {
         _controller = new SprintsController( new PropertyNameTranslator<Sprint, SprintEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         _controller.ControllerContext = _controllerConext.Object;
      }

      private void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( _sessionFactory.Object );
      }

      private static void InitializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
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

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion


      #region Index
      [TestMethod]
      public void Index_ReturnsViewWithAllItems()
      {
         var view = _controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<DomainObjectViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( Sprints.ModelData.Count(), model.Count() );

         foreach (var sprint in Sprints.ModelData)
         {
            Assert.IsNotNull( model.FirstOrDefault( x => x.Id == sprint.Id ) );
         }
      }
      #endregion


      #region Create GET
      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as SprintEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemStatusList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as SprintEditorViewModel;

         Assert.AreEqual( SprintStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.Statuses.Count() );
         foreach (var item in model.Statuses)
         {
            var status = SprintStatuses.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesProjectList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as SprintEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.StatusCd == 'A' && x.Status.IsActive ), model.Projects.Count() );

         foreach (var item in model.Projects)
         {
            var project = Projects.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var viewModel = ((ViewResult)_controller.Create()).Model as SprintEditorViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_AddsCallingActionAndId_IfSpecified()
      {
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.Create( "Edit", parentId.ToString() )).Model as SprintEditorViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var parentId = Guid.NewGuid();

         _controller.Create( "Index" );
         var viewModel = ((ViewResult)_controller.Create( "Edit", parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var parentId = Guid.NewGuid();

         _controller.Create( "Index" );
         _controller.Create( "Edit", parentId.ToString() );
         _controller.Create( "Edit", parentId.ToString() );
         _controller.Create( "Index" );
         _controller.Create( "Index" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 3, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void CreateGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var parentId = Guid.NewGuid();

         _controller.Create( "Index" );
         _controller.Create( "Edit", parentId.ToString() );
         var viewModel = ((ViewResult)_controller.Create()).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }
      #endregion


      #region Create POST Tests
      [TestMethod]
      public void CreatePost_SavesModelIfModelValid()
      {
         var viewModel = CreateSprintEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<Sprint>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( viewModel.Name, items[0].Name );
         Assert.AreEqual( viewModel.Description, items[0].Description );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var viewModel = CreateSprintEditorViewModel();

         var result = _controller.Create( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotSaveModelIfModelIsNotValid()
      {
         var viewModel = CreateSprintEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<Sprint>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 0, items.Count );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var viewModel = CreateSprintEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( viewModel, result.Model );
      }

      //[TestMethod]
      //public void CreatePost_InitializesWorkItemStatusList_ActiveItemSelected()
      //{
      //   var viewModel = CreateSprintEditorViewModel();

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

      //   var returnedModel = result.Model as WorkItemEditorViewModel;

      //   Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), returnedModel.Statuses.Count() );
      //   foreach (var item in returnedModel.Statuses)
      //   {
      //      var itemId = new Guid( item.Value );
      //      var status = WorkItemStatuses.ModelData.First( x => x.Id == itemId );
      //      Assert.AreEqual( status.Name, item.Text );
      //      Assert.IsTrue( (itemId == viewModel.StatusId) ? item.Selected : !item.Selected );
      //   }
      //}

      //[TestMethod]
      //public void CreatePost_InitializesWorkItemTypeList_ActiveItemSelected()
      //{
      //   var viewModel = CreateSprintEditorViewModel();

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

      //   var returnedModel = result.Model as WorkItemEditorViewModel;

      //   Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), returnedModel.WorkItemTypes.Count() );
      //   foreach (var item in returnedModel.WorkItemTypes)
      //   {
      //      var itemId = new Guid( item.Value );
      //      var workItemType = WorkItemTypes.ModelData.First( x => x.Id == itemId );
      //      Assert.AreEqual( workItemType.Name, item.Text );
      //      Assert.IsTrue( (itemId == viewModel.WorkItemTypeId) ? item.Selected : !item.Selected );
      //   }
      //}

      //[TestMethod]
      //public void CreatePost_InitializesProjectList_ActiveItemSelected()
      //{
      //   var viewModel = CreateSprintEditorViewModel();

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

      //   var returnedModel = result.Model as WorkItemEditorViewModel;

      //   Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ), returnedModel.Projects.Count() );
      //   for (int i = 0; i < returnedModel.Projects.Count(); i++)
      //   {
      //      var item = returnedModel.Projects.ElementAt( i );
      //      var itemId = new Guid( item.Value );
      //      var project = Projects.ModelData.First( x => x.Id == itemId );
      //      Assert.AreEqual( project.Name, item.Text );
      //      Assert.IsTrue( (itemId == viewModel.ProjectId) ? item.Selected : !item.Selected );
      //   }
      //}

      //[TestMethod]
      //public void CreatePost_InitializesAssignedToUserList_ActiveItemSelected()
      //{
      //   var viewModel = CreateSprintEditorViewModel();

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

      //   var returnedModel = result.Model as WorkItemEditorViewModel;

      //   Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, returnedModel.AssignedToUsers.Count() );
      //   for (int i = 1; i < returnedModel.AssignedToUsers.Count(); i++)
      //   {
      //      var item = returnedModel.AssignedToUsers.ElementAt( i );
      //      var itemId = new Guid( item.Value );
      //      var user = Users.ModelData.First( x => x.Id == itemId );
      //      Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
      //      Assert.IsTrue( (itemId == viewModel.AssignedToUserId) ? item.Selected : !item.Selected );
      //   }
      //}

      //[TestMethod]
      //public void CreatePost_CopiesMessagesToModelStateIfValidationFails()
      //{
      //   var viewModel = CreateSprintEditorViewModel();

      //   viewModel.Name = "";
      //   var result = _controller.Create( viewModel, _principal.Object );

      //   Assert.AreEqual( 1, _controller.ModelState.Count );
      //   Assert.IsTrue( _controller.ModelState.ContainsKey( "Name" ) );
      //   Assert.IsTrue( result is ViewResult );
      //}

      //[TestMethod]
      //public void CreatePost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      //{
      //   var viewModel = CreateSprintEditorViewModel();

      //   var result = _controller.Create( viewModel, _principal.Object );

      //   Assert.AreEqual( 0, _controller.ModelState.Count );
      //   Assert.IsNotNull( result );
      //   Assert.IsTrue( result is RedirectToRouteResult );
      //}

      //[TestMethod]
      //public void CreatePost_SetsLastModifiedAndCreatedByUserIdToCurrentUser()
      //{
      //   var viewModel = CreateSprintEditorViewModel();

      //   var user = Users.ModelData[0];
      //   _userIdentity
      //      .Setup( x => x.Name )
      //      .Returns( user.UserName );

      //   _controller.Create( viewModel, _principal.Object );

      //   _session.Clear();
      //   var items = _session.Query<WorkItem>()
      //      .Where( x => x.Name == viewModel.Name )
      //      .ToList();

      //   Assert.AreEqual( 1, items.Count );
      //   Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
      //   Assert.AreEqual( user.Id, items[0].CreatedByUser.Id );
      //}

      //[TestMethod]
      //public void CreatePost_SetsAssignedToUserIdToDefault_IfAssignmentsNotAllowedForType()
      //{
      //   var viewModel = CreateSprintEditorViewModel();
      //   viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => !x.IsTask && x.StatusCd == 'A' ).Id;

      //   _controller.Create( viewModel, _principal.Object );

      //   _session.Clear();
      //   var items = _session.Query<WorkItem>()
      //      .Where( x => x.Name == viewModel.Name )
      //      .ToList();

      //   Assert.AreEqual( 1, items.Count );
      //   Assert.IsNull( items[0].AssignedToUser );
      //}

      //[TestMethod]
      //public void CreatePost_DoesNotSetAssignedToUserIdToDefault_IfAssignmentsIsAllowedForType()
      //{
      //   var viewModel = CreateSprintEditorViewModel();
      //   viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.IsTask && x.StatusCd == 'A' ).Id;

      //   _controller.Create( viewModel, _principal.Object );

      //   _session.Clear();
      //   var items = _session.Query<WorkItem>()
      //      .Where( x => x.Name == viewModel.Name )
      //      .ToList();

      //   Assert.AreEqual( 1, items.Count );
      //   Assert.AreEqual( viewModel.AssignedToUserId, items[0].AssignedToUser.Id );
      //}
      #endregion


      #region Details GET
      [TestMethod]
      public void DetailsGet_ReturnsViewWithModel()
      {
         var model = Sprints.ModelData[2];

         var view = _controller.Details( model.Id ) as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( SprintViewModel ) );
         Assert.AreEqual( model.Id, ((SprintViewModel)view.Model).Id );
         Assert.AreEqual( model.Name, ((SprintViewModel)view.Model).Name );
         Assert.AreEqual( model.Description, ((SprintViewModel)view.Model).Description );
      }

      [TestMethod]
      public void DetailsGet_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void DetailsGet_AddsCallingActionAndId_IfSpecified()
      {
         var id = Sprints.ModelData[2].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.Details( id, "Edit", parentId.ToString() )).Model as SprintViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_LeavesCallingActionAndIdAsDefault_IfNotSpecified()
      {
         var id = Sprints.ModelData[2].Id;

         var viewModel = ((ViewResult)_controller.Details( id )).Model as SprintViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, "Index" );
         var viewModel = ((ViewResult)_controller.Details( id, "Edit", parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, "Index" );
         _controller.Details( id, "Edit", parentId.ToString() );
         _controller.Details( id, "Edit", parentId.ToString() );
         _controller.Details( id, "Index" );
         _controller.Details( id, "Index" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 3, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void DetailsGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, "Index" );
         _controller.Details( id, "Edit", parentId.ToString() );
         var viewModel = ((ViewResult)_controller.Details( id )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }
      #endregion


      #region Private Helpers
      private SprintEditorViewModel CreateSprintEditorViewModel()
      {
         return new SprintEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "New Work Item",
            Description = "This is a test",
            StatusId = SprintStatuses.ModelData.First( x => x.StatusCd == 'A' ).Id,
            StatusName = SprintStatuses.ModelData.First( x => x.StatusCd == 'A' ).Name,
            ProjectId = Projects.ModelData.First( x => x.Status.IsActive && x.Status.StatusCd == 'A' ).Id,
            ProjectName = Projects.ModelData.First( x => x.Status.IsActive && x.Status.StatusCd == 'A' ).Name,
            StartDate = new DateTime( 2013, 4, 1 ),
            EndDate = new DateTime( 2013, 4, 30 )
         };
      }
      #endregion
   }
}
