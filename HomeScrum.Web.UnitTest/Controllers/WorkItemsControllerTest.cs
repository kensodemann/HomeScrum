using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.WorkItems;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
      private static Mock<IRepository<WorkItemStatus>> _workItemStatusRepository;
      private static Mock<IRepository<WorkItemType>> _workItemTypeRepository;
      private static Mock<IRepository<Project>> _projectRepository;
      private static Mock<IUserRepository> _userRepository;
      private static MoqMockingKernel _iocKernel;

      private Mock<IValidator<WorkItem>> _validator;
      private Mock<IWorkItemRepository> _workItemRepository;
      private Mock<ILogger> _logger;
      private WorkItemsController _controller;

      private User _currentUser;
      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

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
         SetupWorkItemRepository();
         SetupLogger();

         CreateController();
      }
      #endregion


      #region Index Tests
      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( IEnumerable<WorkItemViewModel> ) );
      }

      [TestMethod]
      public void Index_GetsAllItems()
      {
         _controller.Index();

         _workItemRepository.Verify( x => x.GetAll(), Times.Once() );
      }
      #endregion


      #region Details Tests
      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = WorkItems.ModelData[2];

         _workItemRepository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var view = _controller.Details( model.Id ) as ViewResult;

         _workItemRepository.Verify( x => x.Get( model.Id ), Times.Once() );

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
         var id = Guid.NewGuid();

         _workItemRepository.Setup( x => x.Get( id ) ).Returns( null as WorkItem );

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
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
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.Statuses.Count() );
         foreach (var item in model.Statuses)
         {
            var status = WorkItemStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
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
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesProjectList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.StatusCd == 'A' && x.Status.IsActive ) + 1, model.Projects.Count() );

         bool isFirst = true;
         foreach (var item in model.Projects)
         {
            if (isFirst)
            {
               Assert.AreEqual( default( Guid ).ToString(), item.Value );
               Assert.AreEqual( DisplayStrings.NotAssigned, item.Text );
               Assert.IsFalse( item.Selected );
               isFirst = false;
            }
            else
            {
               var project = Projects.ModelData.First( x => x.Id.ToString() == item.Value );
               Assert.AreEqual( project.Name, item.Text );
               Assert.IsFalse( item.Selected );
            }
         }
      }

      [TestMethod]
      public void CreateGet_InitializesUserList_NothingSelected()
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
               Assert.IsFalse( item.Selected );
            }
            else
            {
               var user = Users.ModelData.First( x => x.Id.ToString() == item.Value );
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
               var workItem = WorkItems.ModelData.First( x => x.Id.ToString() == item.Value );
               Assert.AreEqual( workItem.Name, item.Text );
               Assert.IsFalse( item.Selected );
            }
         }
      }
      #endregion


      #region Create POST Tests
      [TestMethod]
      public void CreatePost_CallRepositoryInsertIfModelValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.Edit( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Update( It.Is<WorkItem>( p => p.Name == viewModel.Name && p.Description == viewModel.Description ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         var result = _controller.Create( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotCallRepositoryAddIfModelIsNotValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Add( It.IsAny<WorkItem>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( viewModel, result.Model );
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
            var status = WorkItemStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (item.Value == viewModel.StatusId.ToString()) ? item.Selected : !item.Selected );
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
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsTrue( (item.Value == viewModel.WorkItemTypeId.ToString()) ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_InitializesProjectList_ActiveItemSelected()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ) + 1, returnedModel.Projects.Count() );
         for (int i = 1; i < returnedModel.Projects.Count(); i++)
         {
            var item = returnedModel.Projects.ElementAt( i );
            var project = Projects.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsTrue( (item.Value == viewModel.ProjectId.ToString()) ? item.Selected : !item.Selected );
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
            var user = Users.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
            Assert.IsTrue( (item.Value == viewModel.AssignedToUserId.ToString()) ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void CreatePost_PassesModelToValidator()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _validator.Verify( x => x.ModelIsValid( It.Is<WorkItem>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), TransactionType.Insert ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var viewModel = CreateWorkItemEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<WorkItem>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

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
         var viewModel = CreateWorkItemEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<WorkItem>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void CreatePost_SetsLastModifiedUserIdToCurrentUser()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _userIdentity.Verify();
         _userRepository.Verify();
         _workItemRepository.Verify( x => x.Add( It.Is<WorkItem>( p => p.Id == viewModel.Id && p.LastModifiedUserRid == _currentUser.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_SetsCreatedByUserIdToCurrentUser()
      {
         var viewModel = CreateWorkItemEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _userIdentity.Verify();
         _userRepository.Verify();
         _workItemRepository.Verify( x => x.Add( It.Is<WorkItem>( w => w.CreatedByUser.Id == _currentUser.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_SetsAssignedToUserIdToDefault_IfAssignmentsNotAllowedForType()
      {
         var viewModel = CreateWorkItemEditorViewModel();
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => !x.IsTask && x.StatusCd == 'A' ).Id;

         _controller.Create( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Add( It.Is<WorkItem>( w => w.AssignedToUser == null ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_DoesNotSetAssignedToUserIdToDefault_IfAssignmentsIsAllowedForType()
      {
         var viewModel = CreateWorkItemEditorViewModel();
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.IsTask && x.StatusCd == 'A' ).Id;

         _controller.Create( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Add( It.Is<WorkItem>( w => w.AssignedToUser.Id == viewModel.AssignedToUserId ) ), Times.Once() );
      }
      #endregion


      #region Edit GET Tests
      [TestMethod]
      public void EditGet_CallsRepositoryGet()
      {
         Guid id = Guid.NewGuid();
         _controller.Edit( id );

         _workItemRepository.Verify( x => x.Get( id ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var model = WorkItems.ModelData[3];
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

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
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var status = WorkItemStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id.ToString() != item.Value && !item.Selected) ||
                           (model.Status.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesWorkItemTypes_WorkItemTypeSelected()
      {
         var model = WorkItems.ModelData.First( x => x.WorkItemType != null && x.WorkItemType.StatusCd == 'A' );
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.WorkItemTypes.Count() );
         foreach (var item in viewModel.WorkItemTypes)
         {
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsTrue( (model.WorkItemType.Id.ToString() != item.Value && !item.Selected) ||
                           (model.WorkItemType.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesProjects_ProjectSelected()
      {
         var model = WorkItems.ModelData.First( x => x.Project != null && x.Project.Status.IsActive && x.Project.Status.StatusCd == 'A' );
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ) + 1, viewModel.Projects.Count() );
         //
         // Skip the first item (null item) 
         for (int i = 1; i < viewModel.Projects.Count(); i++)
         {
            var item = viewModel.Projects.ElementAt( i );
            var project = Projects.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsTrue( (model.Project.Id.ToString() != item.Value && !item.Selected) ||
                           (model.Project.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesAssignedToUsers_UserSelected()
      {
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null && x.AssignedToUser.StatusCd == 'A' );
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, viewModel.AssignedToUsers.Count() );
         //
         // Skip the first item (null item) 
         for (int i = 1; i < viewModel.AssignedToUsers.Count(); i++)
         {
            var item = viewModel.AssignedToUsers.ElementAt( i );
            var user = Users.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
            Assert.IsTrue( (model.AssignedToUser.Id.ToString() != item.Value && !item.Selected) ||
                           (model.AssignedToUser.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesProductBacklog_ParentWorkItemSelected()
      {
         var model = WorkItems.ModelData.First( x => x.ParentWorkItem != null );
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItems.ModelData.Count( x => !x.WorkItemType.IsTask && x.WorkItemType.StatusCd == 'A' && x.Status.IsOpenStatus && x.Status.StatusCd == 'A' ) + 1, viewModel.ProductBacklogItems.Count() );
         for (int i = 0; i<viewModel.ProductBacklogItems.Count();i++)
         {
            var item = viewModel.ProductBacklogItems.ElementAt( i );
            if (i == 0)
            {
               Assert.AreEqual( default( Guid ).ToString(), item.Value );
            }
            else
            {
               var workItem = WorkItems.ModelData.First( x => x.Id.ToString() == item.Value );
               Assert.AreEqual( workItem.Name, item.Text );
               Assert.IsTrue( (model.ParentWorkItem.Id.ToString() != item.Value && !item.Selected) ||
                              (model.ParentWorkItem.Id.ToString() == item.Value && item.Selected) );
            }
         }
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      {
         _workItemRepository.Setup( x => x.Get( It.IsAny<Guid>() ) ).Returns( null as WorkItem );

         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }
      #endregion


      #region Edit POST Tests
      [TestMethod]
      public void EditPost_CallsRepositoryUpdateIfModelValid()
      {
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.Edit( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Update( It.Is<WorkItem>( p => p.Id == model.Id ) ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      {
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         _controller.Edit( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Update( It.IsAny<WorkItem>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

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
         var model = WorkItems.ModelData[2];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( WorkItemEditorViewModel ) );
         Assert.AreEqual( model.Id, ((WorkItemEditorViewModel)result.Model).Id );
         Assert.AreEqual( model.Name, ((WorkItemEditorViewModel)result.Model).Name );
         Assert.AreEqual( model.Description, ((WorkItemEditorViewModel)result.Model).Description );
      }

      [TestMethod]
      public void EditPost_PassesModelToValidator()
      {
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.Edit( viewModel, _principal.Object );

         _validator.Verify( x => x.ModelIsValid( It.Is<WorkItem>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), TransactionType.Update ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<WorkItem>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

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
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<WorkItem>( p => p.Id == viewModel.Id && p.Name == viewModel.Name && p.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditPost_SetsLastModifiedUserId()
      {
         var model = WorkItems.ModelData[3];
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.Edit( viewModel, _principal.Object );

         _userIdentity.Verify();
         _userRepository.Verify();
         _workItemRepository.Verify( x => x.Update( It.Is<WorkItem>( w => w.Id == model.Id && w.LastModifiedUserRid == _currentUser.Id ) ), Times.Once() );
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
            var status = WorkItemStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id.ToString() != item.Value && !item.Selected) ||
                           (model.Status.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_ReInitializesWorkItemTypesIfModelNotValid_WorkItemTypeSelected()
      {
         var model = WorkItems.ModelData.First( x => x.WorkItemType != null && x.WorkItemType.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.WorkItemTypes.Count() );
         foreach (var item in viewModel.WorkItemTypes)
         {
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsTrue( (model.WorkItemType.Id.ToString() != item.Value && !item.Selected) ||
                           (model.WorkItemType.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_ReInitializesProjectsIfModelNotValid_ProjectSelected()
      {
         var model = WorkItems.ModelData.First( x => x.Project != null && x.Project.Status.IsActive && x.Project.Status.StatusCd == 'A' );
         var viewModel = CreateWorkItemEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ) + 1, viewModel.Projects.Count() );
         //
         // Skip the first item (null item) 
         for (int i = 1; i < viewModel.Projects.Count(); i++)
         {
            var item = viewModel.Projects.ElementAt( i );
            var project = Projects.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( project.Name, item.Text );
            Assert.IsTrue( (model.Project.Id.ToString() != item.Value && !item.Selected) ||
                           (model.Project.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_ReInitializesAssignedToUsersIfModelNotValid_UserSelected()
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
            var user = Users.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
            Assert.IsTrue( (model.AssignedToUser.Id.ToString() != item.Value && !item.Selected) ||
                           (model.AssignedToUser.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditPost_SetsAssignedToUserIdToDefault_IfAssignmentsNotAllowedForType()
      {
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null );
         var viewModel = CreateWorkItemEditorViewModel( model );
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => !x.IsTask && x.StatusCd == 'A' ).Id;

         _controller.Edit( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Update( It.Is<WorkItem>( w => w.AssignedToUser == null ) ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotSetAssignedToUserIdToDefault_IfAssignmentsIsAllowedForType()
      {
         var model = WorkItems.ModelData.First( x => x.AssignedToUser != null );
         var viewModel = CreateWorkItemEditorViewModel( model );
         viewModel.WorkItemTypeId = WorkItemTypes.ModelData.First( x => x.IsTask && x.StatusCd == 'A' ).Id;

         _controller.Edit( viewModel, _principal.Object );

         _workItemRepository.Verify( x => x.Update( It.Is<WorkItem>( w => w.AssignedToUser.Id == viewModel.AssignedToUserId ) ), Times.Once() );
      }
      #endregion


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
         _workItemStatusRepository = _iocKernel.GetMock<IRepository<WorkItemStatus>>();
         _workItemStatusRepository.Setup( x => x.GetAll() ).Returns( WorkItemStatuses.ModelData );
         foreach (var model in WorkItemStatuses.ModelData)
         {
            _workItemStatusRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }

         _workItemTypeRepository = _iocKernel.GetMock<IRepository<WorkItemType>>();
         _workItemTypeRepository.Setup( x => x.GetAll() ).Returns( WorkItemTypes.ModelData );
         foreach (var model in WorkItemTypes.ModelData)
         {
            _workItemTypeRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }

         _projectRepository = _iocKernel.GetMock<IRepository<Project>>();
         _projectRepository.Setup( x => x.GetAll() ).Returns( Projects.ModelData );
         foreach (var model in Projects.ModelData)
         {
            _projectRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }

         _userRepository = _iocKernel.GetMock<IUserRepository>();
         _userRepository.Setup( x => x.GetAll() ).Returns( Users.ModelData );
         foreach (var model in Users.ModelData)
         {
            _userRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }
      }

      private static void InitializeTestData()
      {
         Users.CreateTestModelData( initializeIds: true );
         WorkItemStatuses.CreateTestModelData( initializeIds: true );
         WorkItemTypes.CreateTestModelData( initializeIds: true );
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         Projects.CreateTestModelData( initializeIds: true );
         AcceptanceCriteriaStatuses.CreateTestModelData( initializeIds: true );
      }

      ICollection<KeyValuePair<string, string>> CreateStockErrorMessages()
      {
         var messages = new List<KeyValuePair<string, string>>();

         messages.Add( new KeyValuePair<string, string>( "Name", "Name is not unique" ) );
         messages.Add( new KeyValuePair<string, string>( "SomethingElse", "Another Message" ) );

         return messages;
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
         _userRepository
            .Setup( x => x.Get( _currentUser.UserName ) )
            .Returns( _currentUser );
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( _currentUser.UserName );
      }

      private void CreateController()
      {
         _controller = new WorkItemsController( _workItemRepository.Object, _workItemStatusRepository.Object, _workItemTypeRepository.Object,
            _projectRepository.Object, _userRepository.Object, _validator.Object, new WorkItemPropertyNameTranslator(), _logger.Object );
         _controller.ControllerContext = new ControllerContext();
      }

      private void SetupWorkItemRepository()
      {
         WorkItems.CreateTestModelData( initializeIds: true );
         _workItemRepository = new Mock<IWorkItemRepository>();
         _workItemRepository.Setup( x => x.GetAll() ).Returns( WorkItems.ModelData );
         _workItemRepository.Setup( x => x.GetOpenProductBacklog() ).Returns( WorkItems.ModelData.Where( x => !x.WorkItemType.IsTask && x.Status.IsOpenStatus ).ToList() );
      }

      private void SetupLogger()
      {
         _logger = new Mock<ILogger>();
      }

      private void SetupValidator()
      {
         _validator = new Mock<IValidator<WorkItem>>();
         _validator.Setup( x => x.ModelIsValid( It.IsAny<WorkItem>(), It.IsAny<TransactionType>() ) ).Returns( true );
      }
      #endregion
   }
}
