using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
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

      private User _currentUser;
      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();

         CreateMockIOCKernel();
         IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         Database.Build();
         Users.Load();
         WorkItemStatuses.Load();
         WorkItemTypes.Load();
         ProjectStatuses.Load();
         Projects.Load();
         AcceptanceCriteriaStatuses.Load();
         WorkItems.Load();

         SetupCurrentUser();
         SetupLogger();
      }
      #endregion


      #region Index Tests
      [TestMethod]
      public void Index_ReturnsViewWithAllItems()
      {
         var controller = CreateController();

         var view = controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<WorkItemIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( WorkItems.ModelData.Count(), model.Count() );
      }
      #endregion


      #region Details Tests
      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[2];

         var view = controller.Details( model.Id ) as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( WorkItemViewModel ) );
         Assert.AreEqual( model.Id, ((WorkItemViewModel)view.Model).Id );
         Assert.AreEqual( model.Name, ((WorkItemViewModel)view.Model).Name );
         Assert.AreEqual( model.Description, ((WorkItemViewModel)view.Model).Description );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var controller = CreateController();
         var id = Guid.NewGuid();

         var result = controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }
      #endregion


      #region Create GET Tests
      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as WorkItemEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemStatusList_NothingSelected()
      {
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.Statuses.Count() );
         foreach (var item in model.Statuses)
         {
            var status = WorkItemStatuses.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemTypeList_NothingSelected()
      {
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

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
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.StatusCd == 'A' && x.Status.IsActive ), model.Projects.Count() );

         foreach (var item in model.Projects)
         {
            var project = Projects.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesUserList_UassignedSelected()
      {
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

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
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItems.ModelData.Count( x => !x.WorkItemType.IsTask && x.WorkItemType.StatusCd == 'A' && x.Status.IsOpenStatus && x.Status.StatusCd == 'A' ) + 1, model.ProductBacklogItems.Count() );
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
      #endregion


      #region Create POST Tests
      [TestMethod]
      public void CreatePost_SavesModelIfModelValid()
      {
         var controller = CreateController();

         var viewModel = CreateWorkItemEditorViewModel();

         controller.Create( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<WorkItem>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( viewModel.Name, items[0].Name );
            Assert.AreEqual( viewModel.Description, items[0].Description );
         }
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateController();

         var viewModel = CreateWorkItemEditorViewModel();

         var result = controller.Create( viewModel, _principal.Object ) as RedirectToRouteResult;

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

         var viewModel = CreateWorkItemEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<WorkItem>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 0, items.Count );
         }
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var controller = CreateController();

         var viewModel = CreateWorkItemEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( viewModel, result.Model );
      }

      [TestMethod]
      public void CreatePost_InitializesWorkItemStatusList_ActiveItemSelected()
      {
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, _principal.Object ) as ViewResult;

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
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, _principal.Object ) as ViewResult;

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
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ) , returnedModel.Projects.Count() );
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
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, _principal.Object ) as ViewResult;

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
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();

         viewModel.Name = "";
         var result = controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 1, controller.ModelState.Count );
         Assert.IsTrue( controller.ModelState.ContainsKey( "Name" ) );
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();

         var result = controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 0, controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void CreatePost_SetsLastModifiedAndCreatedByUserIdToCurrentUser()
      {
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();

         var user = Users.ModelData[0];
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         controller.Create( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<WorkItem>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
            Assert.AreEqual( user.Id, items[0].CreatedByUser.Id );
         }
      }

      [TestMethod]
      public void CreatePost_SetsAssignedToUserIdToDefault_IfAssignmentsNotAllowedForType()
      {
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => !x.IsTask && x.StatusCd == 'A' ).Id;

         controller.Create( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<WorkItem>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.IsNull( items[0].AssignedToUser );
         }
      }

      [TestMethod]
      public void CreatePost_DoesNotSetAssignedToUserIdToDefault_IfAssignmentsIsAllowedForType()
      {
         var controller = CreateController();
         var viewModel = CreateWorkItemEditorViewModel();
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.IsTask && x.StatusCd == 'A' ).Id;

         controller.Create( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<WorkItem>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( viewModel.AssignedToUserId, items[0].AssignedToUser.Id );
         }
      }
      #endregion


      #region Edit GET Tests
      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[3];

         var result = controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         var returnedModel = result.Model as WorkItemEditorViewModel;
         Assert.IsNotNull( returnedModel );
         Assert.AreEqual( model.Id, returnedModel.Id );
      }

      [TestMethod]
      public void EditGet_InitializesWorkItemStatuses_WorkItemStatusSelected()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.Status.StatusCd == 'A' );

         var result = controller.Edit( model.Id ) as ViewResult;
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
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.WorkItemType != null && x.WorkItemType.StatusCd == 'A' );

         var result = controller.Edit( model.Id ) as ViewResult;
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
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.Project != null && x.Project.Status.IsActive && x.Project.Status.StatusCd == 'A' );

         var result = controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ) , viewModel.Projects.Count() );

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
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null && x.AssignedToUser.StatusCd == 'A' );

         var result = controller.Edit( model.Id ) as ViewResult;
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
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.ParentWorkItem != null );

         var result = controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItems.ModelData.Count( x => !x.WorkItemType.IsTask && x.WorkItemType.StatusCd == 'A' && x.Status.IsOpenStatus && x.Status.StatusCd == 'A' ) + 1, viewModel.ProductBacklogItems.Count() );
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
         var controller = CreateController();

         var result = controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }
      #endregion


      #region Edit POST Tests
      [TestMethod]
      public void EditPost_UpdatesModelIfModelValid()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         viewModel.Name += " Modified";
         controller.Edit( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var item = session.Get<WorkItem>( viewModel.Id );
            Assert.AreEqual( viewModel.Name, item.Name );
         }
      }

      [TestMethod]
      public void EditPost_DoesNotUpdateModelIfModelIsNotValid()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var origName = viewModel.Name;
         viewModel.Name += " Modified";
         controller.Edit( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var item = session.Get<WorkItem>( viewModel.Id );
            Assert.AreNotEqual( viewModel.Name, item.Name );
            Assert.AreEqual( origName, item.Name );
         }
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

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
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( WorkItemEditorViewModel ) );
         Assert.AreEqual( model.Id, ((WorkItemEditorViewModel)result.Model).Id );
         Assert.AreEqual( model.Name, ((WorkItemEditorViewModel)result.Model).Name );
         Assert.AreEqual( model.Description, ((WorkItemEditorViewModel)result.Model).Description );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidationFails()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         viewModel.Name = "";
         var result = controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 1, controller.ModelState.Count );
         Assert.IsTrue( controller.ModelState.ContainsKey( "Name" ) );
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         var result = controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 0, controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditPost_SetsLastModifiedUserId()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         var user = Users.ModelData.First( x => x.Id != model.LastModifiedUserRid );
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         controller.Edit( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<WorkItem>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
         }
      }

      [TestMethod]
      public void EditPost_ReInitializesWorkItemStatusesIfModelNotValid_WorkItemStatusSelected()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.Status.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, _principal.Object );

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
      public void EditGet_ReInitializesWorkItemTypesIfModelNotValid_WorkItemTypeSelected()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.WorkItemType != null && x.WorkItemType.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, _principal.Object );

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
      public void EditGet_ReInitializesProjectsIfModelNotValid_ProjectSelected()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.Project != null && x.Project.Status.IsActive && x.Project.Status.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ), viewModel.Projects.Count() );

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
      public void EditGet_ReInitializesAssignedToUsersIfModelNotValid_UserSelected()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null && x.AssignedToUser.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, _principal.Object );

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
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null );
         var viewModel = CreateWorkItemEditorViewModel( model );
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => !x.IsTask && x.StatusCd == 'A' ).Id;

         controller.Edit( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var item = session.Get<WorkItem>( viewModel.Id );
            Assert.IsNull( item.AssignedToUser );
         }
      }

      [TestMethod]
      public void EditPost_DoesNotSetAssignedToUserIdToDefault_IfAssignmentsIsAllowedForType()
      {
         var controller = CreateController();
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null );
         var viewModel = CreateWorkItemEditorViewModel( model );
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.IsTask && x.StatusCd == 'A' ).Id;

         controller.Edit( viewModel, _principal.Object );

         using (var session = Database.OpenSession())
         {
            var item = session.Get<WorkItem>( viewModel.Id );
            Assert.AreEqual( viewModel.AssignedToUserId, item.AssignedToUser.Id );
         }
      }
      #endregion


      #region private helpers
      private static void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( Database.SessionFactory );
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
            ProjectId = Projects.ModelData.First( x => x.Status.IsActive && x.Status.StatusCd == 'A' ).Id,
            ProjectName = Projects.ModelData.First( x => x.Status.IsActive && x.Status.StatusCd == 'A' ).Name
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
            ProjectName = (workItem.Project == null) ? null : workItem.Project.Name
         };
      }

      private void SetupCurrentUser()
      {
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         // In other places where we use a random user, we use the first active one.
         // Use the first inactive user here just to ensure it is a different user.
         _currentUser = Users.ModelData.First( x => x.StatusCd == 'I' );
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( _currentUser.UserName );
      }

      private WorkItemsController CreateController()
      {
         var controller = new WorkItemsController( new WorkItemPropertyNameTranslator(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }

      private void SetupLogger()
      {
         _logger = new Mock<ILogger>();
      }
      #endregion
   }
}
