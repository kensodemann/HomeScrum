using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.WorkItems;
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
   public class WorkItemsControllerTest
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;
      private Mock<ILogger> _logger;

      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;
      private User _user;

      private Mock<ControllerContext> _controllerConext;
      private Stack<NavigationData> _navigationStack;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private WorkItemsController _controller;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();

         IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         //CurrentSessionContext.Bind( Database.SessionFactory.OpenSession() );
         SetupSession();
         CreateMockIOCKernel();

         BuildDatabase();
         SetupCurrentUser();
         SetupLogger();
         SetupControllerContext();

         _controller = CreateController();
      }

      private void BuildDatabase()
      {
         Database.Build( _session );
         WorkItems.Load( _sessionFactory.Object );
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion


      #region Index Tests
      [TestMethod]
      public void Index_ReturnsViewWithAllItems()
      {
         var view = _controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<WorkItemIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.IsTrue( model.Count() > 0 );
         Assert.AreEqual( WorkItems.ModelData.Count(), model.Count() );
      }
      #endregion


      #region MyAssignent Tests
      [TestMethod]
      public void MyAssignment_ReturnsViewWithNoncompletedItemsAssignedToCurrentUser()
      {
         var view = _controller.MyAssignments( _principal.Object ) as ViewResult;
         var model = view.Model as IEnumerable<WorkItemIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.IsTrue( model.Count() > 0 );
         Assert.AreEqual( WorkItems.ModelData
                             .Where( x => x.AssignedToUser != null && x.AssignedToUser.Id == _user.Id && x.Status.Category != WorkItemStatusCategory.Complete )
                             .Count(), model.Count() );
      }
      #endregion


      #region UnassignedBacklog Tests
      [TestMethod]
      public void UnassignedBacklog_ReturnsViewWithOpenBacklogItemsNotAssignedToASprint()
      {
         var view = _controller.UnassignedBacklog() as ViewResult;
         var model = view.Model as IEnumerable<WorkItemIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.IsTrue( model.Count() > 0 );
         Assert.AreEqual( WorkItems.ModelData
                             .Where( x => x.Sprint == null && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.Status.Category != WorkItemStatusCategory.Complete )
                             .Count(), model.Count() );
      }
      #endregion


      #region UnassignedTasks Tests
      [TestMethod]
      public void UnassignedTasks_ReturnsViewWithOpenUnassignedTasks()
      {
         var view = _controller.UnassignedTasks() as ViewResult;
         var model = view.Model as IEnumerable<WorkItemIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.IsTrue( model.Count() > 0 );
         Assert.AreEqual( WorkItems.ModelData
                             .Where( x => x.AssignedToUser == null && x.WorkItemType.Category == WorkItemTypeCategory.Task && x.Status.Category != WorkItemStatusCategory.Complete )
                             .Count(), model.Count() );
      }
      #endregion


      #region UnassignedProblems Tests
      [TestMethod]
      public void UnassignedProblems_ReturnsViewWithOpenUnassignedProblems()
      {
         var view = _controller.UnassignedProblems() as ViewResult;
         var model = view.Model as IEnumerable<WorkItemIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.IsTrue( model.Count() > 0 );
         Assert.AreEqual( WorkItems.ModelData
                             .Where( x => x.AssignedToUser == null && x.WorkItemType.Category == WorkItemTypeCategory.Issue && x.Status.Category != WorkItemStatusCategory.Complete )
                             .Count(), model.Count() );
      }
      #endregion


      #region Create GET Tests
      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as WorkItemEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemStatusList_NothingSelected()
      {
         var expectedStatuses = WorkItemStatuses.ModelData.Where( x => x.StatusCd == 'A' );

         var result = _controller.Create() as ViewResult;
         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( expectedStatuses.Count(), model.Statuses.Count() );
         foreach (var status in expectedStatuses)
         {
            var item = model.Statuses.FirstOrDefault( x => new Guid( x.Value ) == status.Id );
            Assert.AreEqual( status.Name, item.Text );
            Assert.AreEqual( (status.Category != WorkItemStatusCategory.Complete) ? "True" : "False", item.DataAttributes["IsOpenStatus"] );
            Assert.AreEqual( (status.Category != WorkItemStatusCategory.Unstarted) ? "True" : "False", item.DataAttributes["WorkStarted"] );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemTypeList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), model.WorkItemTypes.Count() );
         foreach (var item in model.WorkItemTypes)
         {
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesProjectList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.StatusCd == 'A' && x.Status.Category == ProjectStatusCategory.Active ), model.Projects.Count() );

         foreach (var item in model.Projects)
         {
            var project = Projects.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesSprintList_NotAssignedItemSelected()
      {
         var expectedSprints = Sprints.ModelData.Where( x => x.Status.StatusCd == 'A' && x.Status.Category != SprintStatusCategory.Complete && (!x.Status.BacklogIsClosed || !x.Status.TaskListIsClosed) );

         var result = _controller.Create() as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( expectedSprints.Count() + 1, viewModel.Sprints.Count() );

         foreach (var sprint in expectedSprints)
         {
            var item = viewModel.Sprints.FirstOrDefault( x => new Guid( x.Value ) == sprint.Id );
            Assert.IsNotNull( item );
            Assert.AreEqual( sprint.Name, item.Text );
            Assert.AreEqual( sprint.Project.Id, new Guid( item.DataAttributes["ProjectId"] ) );
            Assert.AreEqual( (sprint.Status.BacklogIsClosed ? "True" : "False"), item.DataAttributes["BacklogIsClosed"] );
            Assert.AreEqual( (sprint.Status.TaskListIsClosed ? "True" : "False"), item.DataAttributes["TaskListIsClosed"] );
            Assert.IsFalse( item.Selected );
         }

         var defaultItem = viewModel.Sprints.FirstOrDefault( x => new Guid( x.Value ) == Guid.Empty );
         Assert.IsNotNull( defaultItem );
         Assert.AreEqual( "<Not Assigned>", defaultItem.Text );
         Assert.AreEqual( Guid.Empty, new Guid( defaultItem.DataAttributes["ProjectId"] ) );
         Assert.AreEqual( "False", defaultItem.DataAttributes["BacklogIsClosed"] );
         Assert.AreEqual( "False", defaultItem.DataAttributes["TaskListIsClosed"] );
         Assert.IsTrue( defaultItem.Selected );
      }

      [TestMethod]
      public void CreateGet_InitializesUserList_UassignedSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, model.AssignedToUsers.Count() );

         for (int i = 0; i < model.AssignedToUsers.Count(); i++)
         {
            var item = model.AssignedToUsers.ElementAt( i );
            if (i == 0)
            {
               Assert.AreEqual( default( Guid ).ToString(), item.Value );
               Assert.AreEqual( DisplayStrings.NotAssigned, item.Text );
               Assert.IsTrue( item.Selected );
            }
            else
            {
               var user = Users.ModelData.First( x => x.Id == new Guid( item.Value ) );
               Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
               Assert.IsFalse( item.Selected );
            }
         }
      }

      [TestMethod]
      public void CreateGet_InitializesProductBacklogList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItems.ModelData.Count( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.WorkItemType.StatusCd == 'A' && x.Status.Category != WorkItemStatusCategory.Complete && x.Status.StatusCd == 'A' ) + 1, model.ProductBacklogItems.Count() );
         for (int i = 0; i < model.ProductBacklogItems.Count(); i++)
         {
            var item = model.ProductBacklogItems.ElementAt( i );
            if (i == 0)
            {
               Assert.AreEqual( default( Guid ).ToString(), item.Value );
            }
            else
            {
               var workItem = WorkItems.ModelData.First( x => x.Id == new Guid( item.Value ) );
               Assert.AreEqual( workItem.Name, item.Text );
               Assert.IsFalse( item.Selected );
            }
         }
      }

      [TestMethod]
      public void CreateGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var viewModel = ((ViewResult)_controller.Create()).Model as WorkItemEditorViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_AddsCallingActionAndId_IfSpecified()
      {
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.Create( callingAction: "Edit", callingId: parentId.ToString() )).Model as WorkItemEditorViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         controller.Create( callingAction: "Index" );
         var viewModel = ((ViewResult)controller.Create( callingController: "Bogus", callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Bogus", navData.Controller );
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
         controller.Create( callingController: "Something", callingAction: "Index" );
         controller.Create( callingController: "Something", callingAction: "Index" );
         controller.Create( callingAction: "Index" );

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "Something", navData.Controller );
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
      public void CreateGet_SelectsProductBacklogItem_IfParentWorkItemIdSpecified()
      {
         var backlogItems = WorkItems.ModelData
            .Where( x => x.Status.Category != WorkItemStatusCategory.Complete && x.Status.StatusCd == 'A' && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.WorkItemType.StatusCd == 'A' )
            .ToList();
         var backlogItemId = backlogItems.ElementAt( 2 ).Id;
         var result = _controller.Create( parentId: backlogItemId.ToString() ) as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( backlogItems.Count() + 1, model.ProductBacklogItems.Count() );
         for (int i = 0; i < model.ProductBacklogItems.Count(); i++)
         {
            var item = model.ProductBacklogItems.ElementAt( i );
            if (i == 0)
            {
               Assert.AreEqual( Guid.Empty.ToString(), item.Value );
               Assert.IsFalse( item.Selected, "Not Assigned should not be selected" );
            }
            else
            {
               var workItem = backlogItems.First( x => x.Id == new Guid( item.Value ) );
               backlogItems.Remove( workItem );
               Assert.AreEqual( workItem.Name, item.Text );
               if (workItem.Id == backlogItemId)
               {
                  Assert.IsTrue( item.Selected, "Backlog Item should be selected" );
               }
               else
               {
                  Assert.IsFalse( item.Selected, "Backlog Item should not be selected" );
               }
            }
         }
      }

      [TestMethod]
      public void CreateGet_SelectsSprint_IfParentAssignedToSprint()
      {
         var backlogItems = WorkItems.ModelData
            .Where( x => x.Status.Category != WorkItemStatusCategory.Complete && x.Status.StatusCd == 'A' && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.WorkItemType.StatusCd == 'A' )
            .ToList();
         var backlogItem = backlogItems.First( x => x.Sprint != null && x.Sprint.Id != Guid.Empty );
         var sprintId = backlogItem.Sprint.Id;

         var result = _controller.Create( parentId: backlogItem.Id.ToString() ) as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         for (int i = 0; i < model.Sprints.Count(); i++)
         {
            var item = model.Sprints.ElementAt( i );
            if (i == 0)
            {
               Assert.AreEqual( Guid.Empty.ToString(), item.Value );
               Assert.IsFalse( item.Selected, "Not Assigned should not be selected" );
            }
            else
            {
               if (new Guid( item.Value ) == sprintId)
               {
                  Assert.IsTrue( item.Selected, "Sprint should be selected" );
               }
               else
               {
                  Assert.IsFalse( item.Selected, "Sprint should not be selected" );
               }
            }
         }

      }

      [TestMethod]
      public void CreateGet_SetsEditModeToCreate()
      {
         var result = _controller.Create() as ViewResult;
         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( EditMode.Create, model.Mode );
      }
      #endregion


      #region Create POST Tests
      [TestMethod]
      public void CreatePost_SavesModelIfModelValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<WorkItem>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( viewModel.Name, items[0].Name );
         Assert.AreEqual( viewModel.Description, items[0].Description );
      }

      [TestMethod]
      public void CreatePost_RedirectsToEditor_IfModelIsValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         var result = _controller.Create( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 2, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Edit", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Assert.AreNotEqual( new Guid( value.ToString() ), Guid.Empty );
      }

      [TestMethod]
      public void CreatePost_DoesNotSaveModelIfModelIsNotValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<WorkItem>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 0, items.Count );
      }

      [TestMethod]
      public void CreatePost_ReturnsToEditorModeCreate_IfModelIsNotValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;
         var vm = result.Model as WorkItemEditorViewModel;

         Assert.IsNotNull( result );
         Assert.AreEqual( viewModel, vm );
         Assert.AreEqual( EditMode.Create, vm.Mode );
      }

      [TestMethod]
      public void CreatePost_InitializesWorkItemStatusList_ActiveItemSelected()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), returnedModel.Statuses.Count() );
         foreach (var item in returnedModel.Statuses)
         {
            var itemId = new Guid( item.Value );
            var status = WorkItemStatuses.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (itemId == viewModel.StatusId) ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_InitializesWorkItemTypeList_ActiveItemSelected()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), returnedModel.WorkItemTypes.Count() );
         foreach (var item in returnedModel.WorkItemTypes)
         {
            var itemId = new Guid( item.Value );
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsTrue( (itemId == viewModel.WorkItemTypeId) ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_InitializesProjectList_ActiveItemSelected()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.Category == ProjectStatusCategory.Active && x.Status.StatusCd == 'A' ), returnedModel.Projects.Count() );
         for (int i = 0; i < returnedModel.Projects.Count(); i++)
         {
            var item = returnedModel.Projects.ElementAt( i );
            var itemId = new Guid( item.Value );
            var project = Projects.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsTrue( (itemId == viewModel.ProjectId) ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_InitializesAssignedToUserList_ActiveItemSelected()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, returnedModel.AssignedToUsers.Count() );
         for (int i = 1; i < returnedModel.AssignedToUsers.Count(); i++)
         {
            var item = returnedModel.AssignedToUsers.ElementAt( i );
            var itemId = new Guid( item.Value );
            var user = Users.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
            Assert.IsTrue( (itemId == viewModel.AssignedToUserId) ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidationFails()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         viewModel.Name = "";
         var result = _controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 1, _controller.ModelState.Count );
         Assert.IsTrue( _controller.ModelState.ContainsKey( "Name" ) );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         var result = _controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
      }

      [TestMethod]
      public void CreatePost_SetsLastModifiedAndCreatedByUserIdToCurrentUser()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         var user = Users.ModelData[0];
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<WorkItem>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
         Assert.AreEqual( user.Id, items[0].CreatedByUser.Id );
      }

      [TestMethod]
      public void CreatePost_SetsAssignedToUserIdToDefault_IfAssignmentsNotAllowedForType()
      {
         var viewModel = CreateWorkItemEditorViewModel();
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.Category == WorkItemTypeCategory.BacklogItem && x.StatusCd == 'A' ).Id;

         _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<WorkItem>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.IsNull( items[0].AssignedToUser );
      }

      [TestMethod]
      public void CreatePost_DoesNotSetAssignedToUserIdToDefault_IfAssignmentsIsAllowedForType()
      {
         var viewModel = CreateWorkItemEditorViewModel();
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.Category != WorkItemTypeCategory.BacklogItem && x.StatusCd == 'A' ).Id;

         _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<WorkItem>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( viewModel.AssignedToUserId, items[0].AssignedToUser.Id );
      }
      #endregion


      #region Edit GET Tests
      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var model = WorkItems.ModelData[3];

         var result = _controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         var returnedModel = result.Model as WorkItemEditorViewModel;
         Assert.IsNotNull( returnedModel );
         Assert.AreEqual( model.Id, returnedModel.Id );
      }

      [TestMethod]
      public void EditGet_InitializesWorkItemStatuses_WorkItemStatusSelected()
      {
         var model = WorkItems.ModelData.First( x => x.Status.StatusCd == 'A' );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var itemId = new Guid( item.Value );
            var status = WorkItemStatuses.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id != itemId && !item.Selected) ||
                           (model.Status.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesWorkItemTypes_WorkItemTypeSelected()
      {
         var model = WorkItems.ModelData.First( x => x.WorkItemType != null && x.WorkItemType.StatusCd == 'A' );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.WorkItemTypes.Count() );
         foreach (var item in viewModel.WorkItemTypes)
         {
            var itemId = new Guid( item.Value );
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsTrue( (model.WorkItemType.Id != itemId && !item.Selected) ||
                           (model.WorkItemType.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesProjects_ProjectSelected()
      {
         var model = WorkItems.ModelData.First( x => x.Project != null && x.Project.Status.Category == ProjectStatusCategory.Active && x.Project.Status.StatusCd == 'A' );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.Category == ProjectStatusCategory.Active && x.Status.StatusCd == 'A' ), viewModel.Projects.Count() );

         for (int i = 0; i < viewModel.Projects.Count(); i++)
         {
            var item = viewModel.Projects.ElementAt( i );
            var itemId = new Guid( item.Value );
            var project = Projects.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsTrue( (model.Project.Id != itemId && !item.Selected) ||
                           (model.Project.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesAssignedToUsers_UserSelected()
      {
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null && x.AssignedToUser.StatusCd == 'A' );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, viewModel.AssignedToUsers.Count() );
         //
         // Skip the first item (null item) 
         for (int i = 1; i < viewModel.AssignedToUsers.Count(); i++)
         {
            var item = viewModel.AssignedToUsers.ElementAt( i );
            var itemId = new Guid( item.Value );
            var user = Users.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
            Assert.IsTrue( (model.AssignedToUser.Id != itemId && !item.Selected) ||
                           (model.AssignedToUser.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesProductBacklog_ParentWorkItemSelected()
      {
         var model = WorkItems.ModelData.First( x => x.ParentWorkItem != null );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItems.ModelData.Count( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.WorkItemType.StatusCd == 'A' && x.Status.Category != WorkItemStatusCategory.Complete && x.Status.StatusCd == 'A' ) + 1, viewModel.ProductBacklogItems.Count() );
         for (int i = 0; i < viewModel.ProductBacklogItems.Count(); i++)
         {
            var item = viewModel.ProductBacklogItems.ElementAt( i );
            if (i == 0)
            {
               Assert.AreEqual( default( Guid ).ToString(), item.Value );
            }
            else
            {
               var itemId = new Guid( item.Value );
               var workItem = WorkItems.ModelData.First( x => x.Id == itemId );
               Assert.AreEqual( workItem.Name, item.Text );
               Assert.IsTrue( (model.ParentWorkItem.Id != itemId && !item.Selected) ||
                              (model.ParentWorkItem.Id == itemId && item.Selected) );
            }
         }
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFound()
      {
         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditGet_PopulatesTaskList_IfChildTasksExist()
      {
         var parentId = WorkItems.ModelData
            .Where( x => x.ParentWorkItem != null )
            .GroupBy( x => x.ParentWorkItem.Id )
            .Select( g => new { Id = g.Key, Count = g.Count() } )
            .OrderBy( x => x.Count )
            .Last().Id;
         var expectedChildWorkItems = WorkItems.ModelData
            .Where( x => x.ParentWorkItem != null && x.ParentWorkItem.Id == parentId );

         var result = _controller.Edit( parentId ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( expectedChildWorkItems.Count(), viewModel.Tasks.Count() );
         foreach (var child in expectedChildWorkItems)
         {
            Assert.IsNotNull( viewModel.Tasks.FirstOrDefault( x => x.Id == child.Id ) );
         }
      }

      [TestMethod]
      public void EditGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var id = WorkItems.ModelData[0].Id;

         var viewModel = ((ViewResult)_controller.Edit( id )).Model as WorkItemEditorViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_AddsCallingActionAndId_IfSpecified()
      {
         var modelId = WorkItems.ModelData[0].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.Edit( modelId, callingAction: "Edit", callingId: parentId.ToString() )).Model as WorkItemEditorViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var controller = CreateController();
         var id = WorkItems.ModelData[3].Id;
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
         var id = WorkItems.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         controller.Edit( id, callingAction: "Index" );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Edit( id, callingController: "Sprints", callingAction: "Index" );
         controller.Edit( id, callingController: "Sprints", callingAction: "Index" );
         controller.Edit( id, callingAction: "Index" );

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "Sprints", navData.Controller );
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
         var id = WorkItems.ModelData[3].Id;
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
      public void EditGet_SetsEditModeToReadOnly()
      {
         var model = WorkItems.ModelData[3];

         var result = _controller.Edit( model.Id ) as ViewResult;
         var returnedModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( EditMode.ReadOnly, returnedModel.Mode );
      }
      #endregion


      #region Edit POST Tests
      [TestMethod]
      public void EditPost_UpdatesModelIfModelValid()
      {
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         viewModel.Name += " Modified";
         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<WorkItem>( viewModel.Id );
         Assert.AreEqual( viewModel.Name, item.Name );
      }

      [TestMethod]
      public void EditPost_DoesNotUpdateModelIfModelIsNotValid()
      {
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var origName = viewModel.Name;
         viewModel.Name += " Modified";
         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<WorkItem>( viewModel.Id );
         Assert.AreNotEqual( viewModel.Name, item.Name );
         Assert.AreEqual( origName, item.Name );
      }

      [TestMethod]
      public void EditPost_ReturnsToEditorModeReadonly_IfModelIsValid()
      {
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         var result = _controller.Edit( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 2, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Edit", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Assert.AreEqual( new Guid( value.ToString() ), model.Id );
      }

      [TestMethod]
      public void EditPost_ReturnsToEditorModeEdit_IfModelIsNotValid()
      {
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object ) as ViewResult;
         var vm = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( model.Id, vm.Id );
         Assert.AreEqual( EditMode.Edit, vm.Mode );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidationFails()
      {
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         viewModel.Name = "";
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 1, _controller.ModelState.Count );
         Assert.IsTrue( _controller.ModelState.ContainsKey( "Name" ) );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
      }

      [TestMethod]
      public void EditPost_SetsLastModifiedUserId()
      {
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         var user = Users.ModelData.First( x => x.Id != model.LastModifiedUserRid );
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<WorkItem>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
      }

      [TestMethod]
      public void EditPost_ReInitializesWorkItemStatusesIfModelNotValid_WorkItemStatusSelected()
      {
         var model = WorkItems.ModelData.First( x => x.Status.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var itemId = new Guid( item.Value );
            var status = WorkItemStatuses.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id != itemId && !item.Selected) ||
                           (model.Status.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditPost_ReInitializesWorkItemTypesIfModelNotValid_WorkItemTypeSelected()
      {
         var model = WorkItems.ModelData.First( x => x.WorkItemType != null && x.WorkItemType.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.WorkItemTypes.Count() );
         foreach (var item in viewModel.WorkItemTypes)
         {
            var itemId = new Guid( item.Value );
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsTrue( (model.WorkItemType.Id != itemId && !item.Selected) ||
                           (model.WorkItemType.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditPost_ReInitializesProjectsIfModelNotValid_ProjectSelected()
      {
         var model = WorkItems.ModelData.First( x => x.Project != null && x.Project.Status.Category == ProjectStatusCategory.Active && x.Project.Status.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.Category == ProjectStatusCategory.Active && x.Status.StatusCd == 'A' ), viewModel.Projects.Count() );

         for (int i = 1; i < viewModel.Projects.Count(); i++)
         {
            var item = viewModel.Projects.ElementAt( i );
            var itemId = new Guid( item.Value );
            var project = Projects.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsTrue( (model.Project.Id != itemId && !item.Selected) ||
                           (model.Project.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditPost_ReInitializesAssignedToUsersIfModelNotValid_UserSelected()
      {
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null && x.AssignedToUser.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, viewModel.AssignedToUsers.Count() );
         //
         // Skip the first item (null item) 
         for (int i = 1; i < viewModel.AssignedToUsers.Count(); i++)
         {
            var item = viewModel.AssignedToUsers.ElementAt( i );
            var itemId = new Guid( item.Value );
            var user = Users.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
            Assert.IsTrue( (model.AssignedToUser.Id != itemId && !item.Selected) ||
                           (model.AssignedToUser.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditPost_SetsAssignedToUserIdToDefault_IfAssignmentsNotAllowedForType()
      {
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null );
         var viewModel = CreateWorkItemEditorViewModel( model );
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.Category == WorkItemTypeCategory.BacklogItem && x.StatusCd == 'A' ).Id;

         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<WorkItem>( viewModel.Id );
         Assert.IsNull( item.AssignedToUser );
      }

      [TestMethod]
      public void EditPost_DoesNotSetAssignedToUserIdToDefault_IfAssignmentsIsAllowedForType()
      {
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null );
         var viewModel = CreateWorkItemEditorViewModel( model );
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.Category != WorkItemTypeCategory.BacklogItem && x.StatusCd == 'A' ).Id;

         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<WorkItem>( viewModel.Id );
         Assert.AreEqual( viewModel.AssignedToUserId, item.AssignedToUser.Id );
      }

      [TestMethod]
      public void EditPost_ClearsParent_IfParentIsNotAllowedForType()
      {
         var model = WorkItems.ModelData.First( x => x.ParentWorkItem != null && x.ParentWorkItem.Id != default( Guid ) );
         var viewModel = CreateWorkItemEditorViewModel( model );
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.Category == WorkItemTypeCategory.BacklogItem && x.StatusCd == 'A' ).Id;

         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<WorkItem>( viewModel.Id );
         Assert.IsNull( item.ParentWorkItem );
      }

      [TestMethod]
      public void EditPost_SetsProjectInChildTasks()
      {
         var parentId = WorkItems.ModelData.First( x => x.ParentWorkItem != null && x.ParentWorkItem.Id != Guid.Empty ).ParentWorkItem.Id;
         var model = WorkItems.ModelData.Single( x => x.Id == parentId );

         var viewModel = CreateWorkItemEditorViewModel( model );
         var newProjectId = Projects.ModelData.First( x => x.Id != viewModel.ProjectId ).Id;
         viewModel.ProjectId = newProjectId;
         _controller.Edit( viewModel, _principal.Object );

         var children = _session.Query<WorkItem>()
            .Where( x => x.ParentWorkItem != null && x.ParentWorkItem.Id == parentId );
         foreach (var child in children)
         {
            Assert.AreEqual( newProjectId, child.Project.Id );
         }
      }

      [TestMethod]
      public void EditPost_SetsSprintInChildTasks()
      {
         var parentId = WorkItems.ModelData.First( x => x.ParentWorkItem != null && x.ParentWorkItem.Id != Guid.Empty ).ParentWorkItem.Id;
         var model = WorkItems.ModelData.Single( x => x.Id == parentId );

         var viewModel = CreateWorkItemEditorViewModel( model );
         var newSprintId = Sprints.ModelData.First( x => x.Id != viewModel.SprintId ).Id;
         viewModel.SprintId = newSprintId;
         _controller.Edit( viewModel, _principal.Object );

         var children = _session.Query<WorkItem>()
            .Where( x => x.ParentWorkItem != null && x.ParentWorkItem.Id == parentId );
         foreach (var child in children)
         {
            Assert.AreEqual( newSprintId, child.Sprint.Id );
         }
      }
      #endregion


      #region Remove Parent Tests
      [TestMethod]
      public void RemoveParent_MakesParentIdNull_IfWorkItemFound()
      {
         var id = WorkItems.ModelData.First( x => x.ParentWorkItem != null ).Id;

         _controller.RemoveParent( id );

         _session.Clear();
         var item = _session.Get<WorkItem>( id );
         Assert.IsNotNull( item );
         Assert.IsNull( item.ParentWorkItem );
      }

      [TestMethod]
      public void RemoveParent_ReturnsRedirectResult_IfWorkItemNotFound()
      {
         var id = Guid.NewGuid();

         var result = _controller.RemoveParent( id );

         Assert.IsInstanceOfType( result, typeof( RedirectToRouteResult ) );
      }

      [TestMethod]
      public void RemoveParent_RedirectsToCallingAction_IfSpecified()
      {
         var id = WorkItems.ModelData.First( x => x.ParentWorkItem != null ).Id;
         var callingId = Guid.NewGuid();

         var result = _controller.RemoveParent( id, "Edit", callingId.ToString() ) as RedirectToRouteResult;

         Assert.AreEqual( 2, result.RouteValues.Count );
         Assert.AreEqual( "Edit", result.RouteValues["action"] );
         Assert.AreEqual( callingId.ToString(), result.RouteValues["id"] );
      }

      [TestMethod]
      public void RemoveParent_RedirectsToIndex_IfNoCallingActionSpecified()
      {
         var id = WorkItems.ModelData.First( x => x.ParentWorkItem != null ).Id;

         var result = _controller.RemoveParent( id ) as RedirectToRouteResult;
         Assert.AreEqual( 1, result.RouteValues.Count );
         Assert.AreEqual( "Index", result.RouteValues["action"] );
      }
      #endregion


      #region private helpers
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

      private WorkItemEditorViewModel CreateWorkItemEditorViewModel()
      {
         return new WorkItemEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "New Work Item",
            Description = "This is a test",
            StatusId = WorkItemStatuses.ModelData.First( x => x.StatusCd == 'A' ).Id,
            StatusName = WorkItemStatuses.ModelData.First( x => x.StatusCd == 'A' ).Name,
            WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.StatusCd == 'A' ).Id,
            WorkItemTypeName = WorkItemTypes.ModelData.First( x => x.StatusCd == 'A' ).Name,
            AssignedToUserId = Users.ModelData.First( x => x.StatusCd == 'A' ).Id,
            AssignedToUserUserName = Users.ModelData.First( x => x.StatusCd == 'A' ).UserName,
            CreatedByUserId = Users.ModelData.First( x => x.StatusCd == 'A' ).Id,
            CreatedByUserUserName = Users.ModelData.First( x => x.StatusCd == 'A' ).UserName,
            ProjectId = Projects.ModelData.First( x => x.Status.Category == ProjectStatusCategory.Active && x.Status.StatusCd == 'A' ).Id,
            ProjectName = Projects.ModelData.First( x => x.Status.Category == ProjectStatusCategory.Active && x.Status.StatusCd == 'A' ).Name
         };
      }

      private WorkItemEditorViewModel CreateWorkItemEditorViewModel( WorkItem workItem )
      {
         return new WorkItemEditorViewModel()
         {
            Id = workItem.Id,
            Name = workItem.Name,
            Description = workItem.Description,
            StatusId = workItem.Status.Id,
            StatusName = workItem.Status.Name,
            WorkItemTypeId = workItem.WorkItemType.Id,
            WorkItemTypeName = workItem.WorkItemType.Name,
            AssignedToUserId = (workItem.AssignedToUser == null) ? default( Guid ) : workItem.AssignedToUser.Id,
            AssignedToUserUserName = (workItem.AssignedToUser == null) ? null : workItem.AssignedToUser.UserName,
            CreatedByUserId = workItem.CreatedByUser.Id,
            CreatedByUserUserName = workItem.CreatedByUser.UserName,
            ProjectId = (workItem.Project == null) ? default( Guid ) : workItem.Project.Id,
            ProjectName = (workItem.Project == null) ? null : workItem.Project.Name,
            ParentWorkItemId = (workItem.ParentWorkItem == null) ? default( Guid ) : workItem.ParentWorkItem.Id,
            ParentWorkItemName = (workItem.ParentWorkItem == null) ? null : workItem.ParentWorkItem.Name,
            SprintId = (workItem.Sprint == null) ? Guid.Empty : workItem.Sprint.Id,
            SprintName = (workItem.Sprint == null) ? null : workItem.Sprint.Name
         };
      }

      private void SetupCurrentUser()
      {
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         // In other places where we use a random user, we use the first active one.
         // Use the next active user.
         var firstActiveUser = Users.ModelData.First( x => x.StatusCd == 'A' );
         _user = Users.ModelData.First( x => x.StatusCd == 'A' && x.Id != firstActiveUser.Id );
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( _user.UserName );
      }

      private WorkItemsController CreateController()
      {
         var controller = new WorkItemsController( new WorkItemPropertyNameTranslator(), _logger.Object, _sessionFactory.Object );
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
      #endregion
   }
}
