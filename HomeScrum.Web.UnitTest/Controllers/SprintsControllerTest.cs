﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Services;
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

      private Mock<ISprintCalendarService> _sprintCalendarService;

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
         _sprintCalendarService = new Mock<ISprintCalendarService>();
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
         WorkItems.Load( _sessionFactory.Object );
      }

      private void CreateController()
      {
         _controller = new SprintsController( new PropertyNameTranslator<Sprint, SprintEditorViewModel>(), _logger.Object, _sessionFactory.Object, _sprintCalendarService.Object );
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
         var model = view.Model as IEnumerable<SprintIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( Sprints.ModelData.Count(), model.Count() );

         foreach (var sprint in Sprints.ModelData)
         {
            Assert.IsNotNull( model.FirstOrDefault( x => x.Id == sprint.Id ) );
         }
      }

      [TestMethod]
      public void Index_SortsByProjectStartDateStatus()
      {
         var view = _controller.Index() as ViewResult;

         var sprints = view.Model as IEnumerable<SprintIndexViewModel>;

         string previousProject = null;
         DateTime? previousStartDate = null;
         int previousStatusSortSeq = 0;
         foreach (var sprint in sprints)
         {
            var model = Sprints.ModelData.First( x => x.Id == sprint.Id );
            if (previousProject != null)
            {
               Assert.IsTrue( String.Compare( previousProject, sprint.ProjectName ) <= 0, "Order by project" );
               if (previousProject == sprint.ProjectName)
               {
                  Assert.IsTrue( previousStartDate <= (sprint.StartDate ?? DateTime.MaxValue), "Order by date within project" );
                  if (sprint.StartDate == previousStartDate)
                  {
                     Assert.IsTrue( previousStatusSortSeq <= model.Status.SortSequence, "Fall back to status sort" );
                  }
               }
            }
            previousStatusSortSeq = model.Status.SortSequence;
            previousStartDate = sprint.StartDate ?? DateTime.MaxValue;
            previousProject = sprint.ProjectName;
         }
      }
      #endregion


      #region Current Sprints
      [TestMethod]
      public void CurrentSprints_ReturnsViewWithCurrentItems()
      {
         var expectedSprints = Sprints.ModelData.Where( x => x.Status.StatusCd == 'A' && x.Status.Category == SprintStatusCategory.Active && (x.EndDate == null || x.EndDate >= DateTime.Now) && x.StartDate != null && x.StartDate <= DateTime.Now );

         var view = _controller.CurrentSprints() as ViewResult;
         var model = view.Model as IEnumerable<SprintIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( expectedSprints.Count(), model.Count() );

         foreach (var sprint in expectedSprints)
         {
            Assert.IsNotNull( model.FirstOrDefault( x => x.Id == sprint.Id ) );
         }
      }

      [TestMethod]
      public void CurrentSprints_SortsByProjectStartDateStatus()
      {
         var view = _controller.CurrentSprints() as ViewResult;

         var sprints = view.Model as IEnumerable<SprintIndexViewModel>;

         string previousProject = null;
         DateTime? previousStartDate = null;
         int previousStatusSortSeq = 0;
         foreach (var sprint in sprints)
         {
            var model = Sprints.ModelData.First( x => x.Id == sprint.Id );
            if (previousProject != null)
            {
               Assert.IsTrue( String.Compare( previousProject, sprint.ProjectName ) <= 0, "Order by project" );
               if (previousProject == sprint.ProjectName)
               {
                  Assert.IsTrue( previousStartDate <= (sprint.StartDate ?? DateTime.MaxValue), "Order by date within project" );
                  if (sprint.StartDate == previousStartDate)
                  {
                     Assert.IsTrue( previousStatusSortSeq <= model.Status.SortSequence, "Fall back to status sort" );
                  }
               }
            }
            previousStatusSortSeq = model.Status.SortSequence;
            previousStartDate = sprint.StartDate ?? DateTime.MaxValue;
            previousProject = sprint.ProjectName;
         }
      }
      #endregion


      #region Open Sprints
      [TestMethod]
      public void OpenSprints_ReturnsViewWithNonCompletedSprints()
      {
         var expectedSprints = Sprints.ModelData.Where( x => x.Status.StatusCd == 'A' && x.Status.Category != SprintStatusCategory.Complete );

         var view = _controller.OpenSprints() as ViewResult;
         var model = view.Model as IEnumerable<SprintIndexViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( expectedSprints.Count(), model.Count() );

         foreach (var sprint in expectedSprints)
         {
            Assert.IsNotNull( model.FirstOrDefault( x => x.Id == sprint.Id ) );
         }
      }

      [TestMethod]
      public void OpenSprints_SortsByProjectStartDateStatus()
      {
         var view = _controller.OpenSprints() as ViewResult;

         var sprints = view.Model as IEnumerable<SprintIndexViewModel>;

         string previousProject = null;
         DateTime? previousStartDate = null;
         int previousStatusSortSeq = 0;
         foreach (var sprint in sprints)
         {
            var model = Sprints.ModelData.First( x => x.Id == sprint.Id );
            if (previousProject != null)
            {
               Assert.IsTrue( String.Compare( previousProject, sprint.ProjectName ) <= 0, "Order by project" );
               if (previousProject == sprint.ProjectName)
               {
                  Assert.IsTrue( (previousStartDate ?? DateTime.MinValue) <= (sprint.StartDate ?? DateTime.MaxValue), "Order by date within project" );
                  if (sprint.StartDate == previousStartDate)
                  {
                     Assert.IsTrue( previousStatusSortSeq <= model.Status.SortSequence, "Fall back to status sort" );
                  }
               }
            }
            previousStatusSortSeq = model.Status.SortSequence;
            previousStartDate = sprint.StartDate; // ?? DateTime.MaxValue;
            previousProject = sprint.ProjectName;
         }
      }
      #endregion


      #region Create GET
      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var result = _controller.Create() as PartialViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as SprintEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemStatusList_NothingSelected()
      {
         var result = _controller.Create() as PartialViewResult;

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
         var result = _controller.Create() as PartialViewResult;

         var model = result.Model as SprintEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.StatusCd == 'A' && x.Status.Category == ProjectStatusCategory.Active ), model.Projects.Count() );

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
         var viewModel = ((PartialViewResult)_controller.Create()).Model as SprintEditorViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_AddsCallingActionAndId_IfSpecified()
      {
         var parentId = Guid.NewGuid();

         var viewModel = ((PartialViewResult)_controller.Create( callingAction: "Edit", callingId: parentId.ToString() )).Model as SprintEditorViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var parentId = Guid.NewGuid();

         _controller.Create( callingAction: "Index" );
         var viewModel = ((PartialViewResult)_controller.Create( callingController: "Bogus", callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
         var parentId = Guid.NewGuid();

         _controller.Create( callingAction: "Index" );
         _controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Create( callingController: "Something", callingAction: "Index" );
         _controller.Create( callingController: "Something", callingAction: "Index" );
         _controller.Create( callingAction: "Index" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
         var parentId = Guid.NewGuid();

         _controller.Create( callingAction: "Index" );
         _controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((PartialViewResult)_controller.Create()).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void CreateGet_SetsModeToCreate()
      {
         var result = _controller.Create() as PartialViewResult;
         var model = result.Model as SprintEditorViewModel;

         Assert.AreEqual( EditMode.Create, model.Mode );
      }
      #endregion


      #region Create POST
      [TestMethod]
      public void CreatePost_SavesModelIfModelValid()
      {
         var viewModel = CreateSprintEditorViewModel();

         _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var model = _session.Query<Sprint>().Single( x => x.Name == viewModel.Name );

         // ViewModel will not have ID on a Create, so set it before the assert
         viewModel.Id = model.Id;
         AssertModelAndViewModelPropertiesEqual( model, viewModel );
      }

      [TestMethod]
      public void CreatePost_RedirectsToCaller_IfModelIsValid()
      {
         var viewModel = CreateSprintEditorViewModel();

         var callingId = Guid.NewGuid();
         _controller.Create( callingAction: "MyStuff", callingController: "Stuff", callingId: callingId.ToString() );
         var result = _controller.Create( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 3, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "MyStuff", value.ToString() );

         result.RouteValues.TryGetValue( "controller", out value );
         Assert.AreEqual( "Stuff", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Guid actualId;
         if (!Guid.TryParse( value.ToString(), out actualId ))
         {
            Assert.Fail( "id was not a GUID" );
         }
         Assert.AreEqual( callingId, actualId );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndex_IfModeIsValidAndNoCaller()
      {
         var viewModel = CreateSprintEditorViewModel();

         var result = _controller.Create( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "index", value.ToString() );
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
      public void CreatePost_ReturnsToEditorModeCreate_IfModelIsNotValid()
      {
         var viewModel = CreateSprintEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;
         var vm = result.Model as SprintEditorViewModel;

         Assert.AreEqual( Guid.Empty, vm.Id );
         Assert.AreEqual( EditMode.Create, vm.Mode );
      }

      [TestMethod]
      public void CreatePost_InitializesSprintStatusListIfError_ActiveItemSelected()
      {
         var viewModel = CreateSprintEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as SprintEditorViewModel;

         Assert.AreEqual( SprintStatuses.ModelData.Count( x => x.StatusCd == 'A' ), returnedModel.Statuses.Count() );
         foreach (var item in returnedModel.Statuses)
         {
            var itemId = new Guid( item.Value );
            var status = SprintStatuses.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (itemId == viewModel.StatusId) ? item.Selected : !item.Selected );
         }
      }


      [TestMethod]
      public void CreatePost_InitializesProjectListIfError_ActiveItemSelected()
      {
         var viewModel = CreateSprintEditorViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel, _principal.Object ) as ViewResult;

         var returnedModel = result.Model as SprintEditorViewModel;

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
      public void CreatePost_CopiesMessagesToModelStateIfValidationFails()
      {
         var viewModel = CreateSprintEditorViewModel();

         viewModel.Name = "";
         var result = _controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 1, _controller.ModelState.Count );
         Assert.IsTrue( _controller.ModelState.ContainsKey( "Name" ) );
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var viewModel = CreateSprintEditorViewModel();

         var result = _controller.Create( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
      }

      [TestMethod]
      public void CreatePost_SetsLastModifiedAndCreatedByUserIdToCurrentUser()
      {
         var viewModel = CreateSprintEditorViewModel();
         viewModel.CreatedByUserId = Guid.Empty;
         viewModel.CreatedByUserUserName = null;

         var user = Users.ModelData[0];
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         _controller.Create( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<Sprint>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
         Assert.AreEqual( user.Id, items[0].CreatedByUser.Id );
      }
      #endregion


      #region Details GET Tests
      [TestMethod]
      public void DetailsGet_ReturnsViewWithModel()
      {
         var model = Sprints.ModelData[3];

         var result = _controller.Details( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         var returnedModel = result.Model as SprintViewModel;
         Assert.IsNotNull( returnedModel );
         Assert.AreEqual( model.Id, returnedModel.Id );
      }

      [TestMethod]
      public void DetailsGet_ReturnsNoDataFoundIfModelNotFound()
      {
         var result = _controller.Details( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void DetailsGet_PopulatesTaskList_IfTasksExist()
      {
         var parentId = WorkItems.ModelData
            .Where( x => x.Sprint != null && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .GroupBy( x => x.Sprint.Id )
            .Select( g => new { Id = g.Key, Count = g.Count() } )
            .OrderBy( x => x.Count )
            .Last().Id;
         var expectedChildWorkItems = WorkItems.ModelData
            .Where( x => x.Sprint != null &&
               x.Sprint.Id == parentId &&
               x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem );

         var result = _controller.Details( parentId ) as ViewResult;
         var viewModel = result.Model as SprintViewModel;

         Assert.AreEqual( expectedChildWorkItems.Count(), viewModel.Tasks.Count() );
         foreach (var child in expectedChildWorkItems)
         {
            Assert.IsNotNull( viewModel.Tasks.FirstOrDefault( x => x.Id == child.Id ) );
         }
      }

      [TestMethod]
      public void DetailsGet_PopulatesBacklogList_IfTasksExist()
      {
         var parentId = WorkItems.ModelData
            .Where( x => x.Sprint != null && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem )
            .GroupBy( x => x.Sprint.Id )
            .Select( g => new { Id = g.Key, Count = g.Count() } )
            .OrderBy( x => x.Count )
            .Last().Id;
         var expectedChildWorkItems = WorkItems.ModelData
            .Where( x => x.Sprint != null &&
               x.Sprint.Id == parentId &&
               x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem );

         var result = _controller.Details( parentId ) as ViewResult;
         var viewModel = result.Model as SprintViewModel;

         Assert.AreEqual( expectedChildWorkItems.Count(), viewModel.BacklogItems.Count() );
         foreach (var child in expectedChildWorkItems)
         {
            Assert.IsNotNull( viewModel.BacklogItems.FirstOrDefault( x => x.Id == child.Id ) );
         }
      }

      [TestMethod]
      public void DetailsGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var id = Sprints.ModelData[0].Id;

         var viewModel = ((ViewResult)_controller.Details( id )).Model as SprintViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_AddsCallingActionAndId_IfSpecified()
      {
         var modelId = Sprints.ModelData[0].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.Details( modelId, callingAction: "Edit", callingId: parentId.ToString() )).Model as SprintViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, callingAction: "Index" );
         var viewModel = ((ViewResult)_controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void DetailsGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, callingAction: "Index" );
         _controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Details( id, callingController: "Sprints", callingAction: "Index" );
         _controller.Details( id, callingController: "Sprints", callingAction: "Index" );
         _controller.Details( id, callingAction: "Index" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void DetailsGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, callingAction: "Index" );
         _controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((ViewResult)_controller.Details( id )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void DetailsGet_SetsTotalPointsToTaskItemPoints_IfTasksExist()
      {
         var sprintId = WorkItems.ModelData
            .Where( x => x.Sprint != null && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .GroupBy( x => x.Sprint.Id )
            .Select( g => new { Id = g.Key, Count = g.Count() } )
            .OrderBy( x => x.Count )
            .Last().Id;
         var expectedPoints = WorkItems.ModelData
            .Where( x => x.Sprint != null &&
               x.Sprint.Id == sprintId &&
               x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .Sum( x => x.Points );

         var result = _controller.Details( sprintId ) as ViewResult;
         var viewModel = result.Model as SprintViewModel;

         Assert.AreEqual( expectedPoints, viewModel.TotalPoints );
      }


      [TestMethod]
      public void DetailsGet_LoadsCalendar()
      {
         var sprint = Sprints.ModelData.First( x => x.Calendar.Count > 0 );

         var viewModel = ((ViewResult)_controller.Details( sprint.Id )).Model as SprintViewModel;

         Assert.AreEqual( sprint.Calendar.Count, viewModel.Calendar.Count() );
         foreach (var entry in sprint.Calendar)
         {
            Assert.IsNotNull( viewModel.Calendar.SingleOrDefault( x => x.HistoryDate == entry.HistoryDate && x.PointsRemaining == entry.PointsRemaining ) );
         }
      }

      [TestMethod]
      public void DetailsGet_CreatesEmptyCalendar_IfNoEntriesExistForSprint()
      {
         var sprint = Sprints.ModelData.First( x => x.Calendar.Count == 0 );

         var viewModel = ((ViewResult)_controller.Details( sprint.Id )).Model as SprintViewModel;

         Assert.AreEqual( 0, viewModel.Calendar.Count() );
      }

      [TestMethod]
      public void DetailsGet_GetsPointsForTasks()
      {
         var sprint = WorkItems.ModelData
            .Where( x => x.Sprint != null && x.ParentWorkItem == null && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .GroupBy( x => x.Sprint.Id )
            .Select( g => new { Id = g.Key, TaskCount = g.Count() } )
            .OrderBy( x => x.TaskCount )
            .Last();

         var result = _controller.Details( sprint.Id ) as ViewResult;
         var vm = result.Model as SprintViewModel;

         Assert.AreEqual( sprint.TaskCount, vm.Tasks.Count() );
         foreach (var task in vm.Tasks)
         {
            var model = WorkItems.ModelData.Single( x => x.Id == task.Id );
            Assert.AreEqual( model.Points, task.Points );
            Assert.AreEqual( model.PointsRemaining, task.PointsRemaining );
         }
      }

      [TestMethod]
      public void DetailsGet_SumsPointsForBacklogItems()
      {
         var sprint = WorkItems.ModelData
            .Where( x => x.Sprint != null && x.ParentWorkItem == null && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.Tasks.Count() > 0 )
            .GroupBy( x => x.Sprint.Id )
            .Select( g => new { Id = g.Key, BacklogCount = g.Count() } )
            .OrderBy( x => x.BacklogCount )
            .Last();

         var result = _controller.Details( sprint.Id ) as ViewResult;
         var vm = result.Model as SprintViewModel;

         Assert.AreEqual( sprint.BacklogCount, vm.BacklogItems.Count() );
         foreach (var item in vm.BacklogItems)
         {
            var model = WorkItems.ModelData.Single( x => x.Id == item.Id );
            Assert.AreEqual( model.Tasks.Sum( x => x.Points ), item.Points );
            Assert.AreEqual( model.Tasks.Sum( x => x.PointsRemaining ), item.PointsRemaining );
         }
      }

      [TestMethod]
      public void DetailsGet_CanAddBacklog_IfBacklogNotClosed()
      {
         var id = Sprints.ModelData.First( x => !x.Status.BacklogIsClosed ).Id;

         var result = _controller.Details( id ) as ViewResult;
         var vm = result.Model as SprintViewModel;

         Assert.IsTrue( vm.CanAddBacklog );
      }

      [TestMethod]
      public void DetailsGet_CannotAddBacklog_IfBacklogClosed()
      {
         var id = Sprints.ModelData.First( x => x.Status.BacklogIsClosed ).Id;

         var result = _controller.Details( id ) as ViewResult;
         var vm = result.Model as SprintViewModel;

         Assert.IsFalse( vm.CanAddBacklog );
      }

      [TestMethod]
      public void DetailsGet_CanAddTasks_IfTaskListNotClosed()
      {
         var id = Sprints.ModelData.First( x => !x.Status.TaskListIsClosed ).Id;

         var result = _controller.Details( id ) as ViewResult;
         var vm = result.Model as SprintViewModel;

         Assert.IsTrue( vm.CanAddTasks );
      }

      [TestMethod]
      public void DetailsGet_CannotAddTask_IfTaskListClosed()
      {
         var id = Sprints.ModelData.First( x => x.Status.TaskListIsClosed ).Id;

         var result = _controller.Details( id ) as ViewResult;
         var vm = result.Model as SprintViewModel;

         Assert.IsFalse( vm.CanAddTasks );
      }
      #endregion


      #region Edit GET
      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var model = Sprints.ModelData[3];

         var result = _controller.Edit( model.Id ) as PartialViewResult;

         Assert.IsNotNull( result );
         var returnedModel = result.Model as SprintEditorViewModel;
         Assert.IsNotNull( returnedModel );
         AssertModelAndViewModelPropertiesEqual( model, returnedModel );
      }

      [TestMethod]
      public void EditGet_InitializesSprintStatuses_SprintStatusSelected()
      {
         var model = Sprints.ModelData.First( x => x.Status.StatusCd == 'A' );

         var result = _controller.Edit( model.Id ) as PartialViewResult;
         var viewModel = result.Model as SprintEditorViewModel;

         Assert.AreEqual( SprintStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var itemId = new Guid( item.Value );
            var status = SprintStatuses.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id != itemId && !item.Selected) ||
                           (model.Status.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesProjects_ProjectSelected()
      {
         var model = Sprints.ModelData.First( x => x.Project != null && x.Project.Status.Category == ProjectStatusCategory.Active && x.Project.Status.StatusCd == 'A' );

         var result = _controller.Edit( model.Id ) as PartialViewResult;
         var viewModel = result.Model as SprintEditorViewModel;

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
      public void EditGet_ReturnsNoDataFoundIfModelNotFound()
      {
         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var id = Sprints.ModelData[0].Id;

         var viewModel = ((PartialViewResult)_controller.Edit( id )).Model as SprintEditorViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_AddsCallingActionAndId_IfSpecified()
      {
         var modelId = Sprints.ModelData[0].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((PartialViewResult)_controller.Edit( modelId, callingAction: "Edit", callingId: parentId.ToString() )).Model as SprintEditorViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Edit( id, callingAction: "Index" );
         var viewModel = ((PartialViewResult)_controller.Edit( id, callingController: "Bogus", callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void EditGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Edit( id, callingAction: "Index" );
         _controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Edit( id, callingController: "Projects", callingAction: "Index" );
         _controller.Edit( id, callingController: "Projects", callingAction: "Index" );
         _controller.Edit( id, callingAction: "Index" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "Projects", navData.Controller );
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
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Edit( id, callingAction: "Index" );
         _controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((PartialViewResult)_controller.Edit( id )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void EditGet_SetsModeToEdit()
      {
         var model = Sprints.ModelData[3];

         var result = _controller.Edit( model.Id ) as PartialViewResult;
         var returnedModel = result.Model as SprintEditorViewModel;

         Assert.AreEqual( EditMode.Edit, returnedModel.Mode );
      }

      [TestMethod]
      public void EditGet_CalculatesTotalPoints()
      {
         var sprint = Sprints.ModelData.Where( x => x.Project.Name == "Sandwiches" ).ElementAt( 0 );

         var viewModel = ((PartialViewResult)_controller.Edit( sprint.Id )).Model as SprintEditorViewModel;

         Assert.AreEqual( WorkItems.ModelData
                             .Where( x => x.Sprint != null &&
                                     x.Sprint.Id == sprint.Id &&
                                     x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
                             .Sum( x => x.Points ),
                          viewModel.TotalPoints );
      }
      #endregion


      #region Edit POST
      [TestMethod]
      public void EditPost_UpdatesModelIfModelValid()
      {
         var model = Sprints.ModelData[2];
         var viewModel = CreateSprintEditorViewModel( model );

         viewModel.Name += " Modified";
         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<Sprint>( viewModel.Id );
         AssertModelAndViewModelPropertiesEqual( item, viewModel );
      }

      [TestMethod]
      public void EditPost_DoesNotUpdateModelIfModelIsNotValid()
      {
         var model = Sprints.ModelData[2];
         var viewModel = CreateSprintEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var origName = viewModel.Name;
         viewModel.Name += " Modified";
         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var item = _session.Get<Sprint>( viewModel.Id );
         Assert.AreNotEqual( viewModel.Name, item.Name );
         Assert.AreEqual( origName, item.Name );
      }

      [TestMethod]
      public void EditPost_RedirectsToCaller_IfModelIsValid()
      {
         var model = Sprints.ModelData[2];
         var viewModel = CreateSprintEditorViewModel( model );

         var callingId = Guid.NewGuid();
         _controller.Edit( model.Id, callingAction: "Run", callingController: "FredFlintstone", callingId: callingId.ToString() );
         _session.Clear();
         var result = _controller.Edit( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 3, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Run", value.ToString() );

         result.RouteValues.TryGetValue( "controller", out value );
         Assert.AreEqual( "FredFlintstone", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Guid actualId;
         if (!Guid.TryParse( value.ToString(), out actualId ))
         {
            Assert.Fail( "id was not a GUID" );
         }
         Assert.AreEqual( callingId, actualId );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndex_IfModelIsValidAndNoCaller()
      {
         var model = Sprints.ModelData[2];
         var viewModel = CreateSprintEditorViewModel( model );

         var result = _controller.Edit( viewModel, _principal.Object ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsToEditorModeEdit_IfModelIsNotValid()
      {
         var model = Sprints.ModelData[2];
         var viewModel = CreateSprintEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object ) as ViewResult;
         var vm = result.Model as SprintEditorViewModel;

         Assert.AreEqual( model.Id, vm.Id );
         Assert.AreEqual( EditMode.Edit, vm.Mode );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidationFails()
      {
         var model = Sprints.ModelData[3];
         var viewModel = CreateSprintEditorViewModel( model );

         viewModel.Name = "";
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 1, _controller.ModelState.Count );
         Assert.IsTrue( _controller.ModelState.ContainsKey( "Name" ) );
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var model = Sprints.ModelData[3];
         var viewModel = CreateSprintEditorViewModel( model );

         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( 0, _controller.ModelState.Count );
      }

      [TestMethod]
      public void EditPost_SetsLastModifiedUserId()
      {
         var model = Sprints.ModelData[3];
         var viewModel = CreateSprintEditorViewModel( model );

         var user = Users.ModelData.First( x => x.Id != model.LastModifiedUserRid );
         _userIdentity
            .Setup( x => x.Name )
            .Returns( user.UserName );

         _controller.Edit( viewModel, _principal.Object );

         _session.Clear();
         var items = _session.Query<Sprint>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( user.Id, items[0].LastModifiedUserRid );
      }

      [TestMethod]
      public void EditPost_ReInitializesSprintStatusesIfModelNotValid_SprintStatusSelected()
      {
         var model = Sprints.ModelData.First( x => x.Status.StatusCd == 'A' );
         var viewModel = CreateSprintEditorViewModel( model );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel, _principal.Object );

         Assert.AreEqual( SprintStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var itemId = new Guid( item.Value );
            var status = SprintStatuses.ModelData.First( x => x.Id == itemId );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id != itemId && !item.Selected) ||
                           (model.Status.Id == itemId && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_ReInitializesProjectsIfModelNotValid_ProjectSelected()
      {
         var model = Sprints.ModelData.First( x => x.Project != null && x.Project.Status.Category == ProjectStatusCategory.Active && x.Project.Status.StatusCd == 'A' );
         var viewModel = CreateSprintEditorViewModel( model );

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
      public void EditPost_CallsSprintCalendarService()
      {
         var model = Sprints.ModelData[3];
         var viewModel = CreateSprintEditorViewModel( model );

         var result = _controller.Edit( viewModel, _principal.Object );

         _sprintCalendarService.Verify( x => x.Reset( It.Is<Sprint>( s => s.Id == model.Id ) ), Times.Once() );
      }
      #endregion


      #region AddBacklogItems GET
      [TestMethod]
      public void AddBacklogItemsGet_ReturnsViewWithOpenUnassignedBacklogItemsForProject_PlusItemsForSprint()
      {
         var projectId = Projects.ModelData.First( x => x.Name == "Home Scrum" ).Id;
         var sprintId = Sprints.ModelData.First( x => x.Project.Id == projectId && !x.Status.BacklogIsClosed ).Id;
         var expectedWorkItems = WorkItems.ModelData.Where( x => x.Status.Category != WorkItemStatusCategory.Complete && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.Project.Id == projectId && (x.Sprint == null || x.Sprint.Id == sprintId) );

         var view = _controller.AddBacklogItems( sprintId ) as ViewResult;
         var model = view.Model as WorkItemsListForSprintViewModel;

         Assert.IsNotNull( model );
         Assert.AreEqual( expectedWorkItems.Count(), model.WorkItems.Count() );

         foreach (var workItem in expectedWorkItems)
         {
            var workItemViewModel = model.WorkItems.SingleOrDefault( x => x.Id == workItem.Id );
            Assert.IsNotNull( workItemViewModel );
            Assert.AreEqual( workItem.Name, workItemViewModel.Name );
            Assert.AreEqual( workItem.Status.Name, workItemViewModel.StatusName );
            Assert.AreEqual( workItem.WorkItemType.Name, workItemViewModel.WorkItemTypeName );
            Assert.AreEqual( workItem.Sprint != null, workItemViewModel.IsInTargetSprint );
         }
      }
      [TestMethod]
      public void AddBacklogItemsGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var sprintId = Sprints.ModelData.First().Id;
         var viewModel = ((ViewResult)_controller.AddBacklogItems( sprintId )).Model as WorkItemsListForSprintViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void AddBacklogItemsGet_AddsCallingActionAndId_IfSpecified()
      {
         var sprintId = Sprints.ModelData.First().Id;
         var callingId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.AddBacklogItems( sprintId, "Edit", callingId.ToString() )).Model as WorkItemsListForSprintViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( callingId, viewModel.CallingId );
      }
      #endregion


      #region AddBacklogItems POST
      [TestMethod]
      public void AddBacklogItem_AddsBacklogItemsToSprint()
      {
         RemoveAllWorkItemsFromSprints();

         var sprint = Sprints.ModelData.First( x => x.Project.Name == "Home Scrum" && !x.Status.BacklogIsClosed );

         var viewModel = CreateBacklogItemsForSprintViewModel( sprint );
         viewModel.WorkItems[1].IsInTargetSprint = true;
         viewModel.WorkItems[3].IsInTargetSprint = true;
         viewModel.WorkItems[4].IsInTargetSprint = true;

         _controller.AddBacklogItems( viewModel );

         foreach (var item in viewModel.WorkItems)
         {
            var workItem = _session.Get<WorkItem>( item.Id );
            if (item.IsInTargetSprint)
            {
               Assert.AreEqual( sprint.Id, workItem.Sprint.Id );
            }
            else
            {
               Assert.IsNull( workItem.Sprint );
            }
         }
      }

      [TestMethod]
      public void AddBacklogItem_AddsTasksForBacklogItemToSprint()
      {
         RemoveAllWorkItemsFromSprints();

         var sprint = Sprints.ModelData.First( x => x.Project.Name == "Home Scrum" && !x.Status.BacklogIsClosed );

         var viewModel = CreateBacklogItemsForSprintViewModel( sprint );
         viewModel.WorkItems[0].IsInTargetSprint = true;

         _controller.AddBacklogItems( viewModel );

         foreach (var item in WorkItems.ModelData)
         {
            var workItem = _session.Get<WorkItem>( item.Id );
            if (item.Id == viewModel.WorkItems[0].Id ||
                (item.ParentWorkItem != null && item.ParentWorkItem.Id == viewModel.WorkItems[0].Id))
            {
               Assert.AreEqual( sprint.Id, workItem.Sprint.Id );
            }
            else
            {
               Assert.IsNull( workItem.Sprint );
            }
         }
      }

      [TestMethod]
      public void AddBacklogItem_RedirectsBackToDetails()
      {
         var sprint = Sprints.ModelData.First( x => x.Project.Name == "Home Scrum" && !x.Status.BacklogIsClosed );
         var viewModel = CreateBacklogItemsForSprintViewModel( sprint );

         var result = _controller.AddBacklogItems( viewModel ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 2, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Details", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Assert.AreEqual( value, sprint.Id );
      }
      #endregion


      #region AddTasks GET
      [TestMethod]
      public void AddTasksGet_ReturnsViewWithOpenUnassignedTasksWithoutBacklog_PlusTasksForSprint()
      {
         var projectId = Projects.ModelData.First( x => x.Name == "Home Scrum" ).Id;
         var sprintId = Sprints.ModelData.First( x => x.Project.Id == projectId && !x.Status.TaskListIsClosed ).Id;
         var expectedWorkItems = WorkItems.ModelData.Where( x => x.Status.Category != WorkItemStatusCategory.Complete && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem && x.Project.Id == projectId && x.ParentWorkItem == null && (x.Sprint == null || x.Sprint.Id == sprintId) );

         var view = _controller.AddTasks( sprintId ) as ViewResult;
         var model = view.Model as WorkItemsListForSprintViewModel;

         Assert.IsNotNull( model );
         Assert.AreEqual( expectedWorkItems.Count(), model.WorkItems.Count() );

         foreach (var workItem in expectedWorkItems)
         {
            var workItemViewModel = model.WorkItems.SingleOrDefault( x => x.Id == workItem.Id );
            Assert.IsNotNull( workItemViewModel );
            Assert.AreEqual( workItem.Name, workItemViewModel.Name );
            Assert.AreEqual( workItem.Status.Name, workItemViewModel.StatusName );
            Assert.AreEqual( workItem.WorkItemType.Name, workItemViewModel.WorkItemTypeName );
            Assert.AreEqual( workItem.Sprint != null, workItemViewModel.IsInTargetSprint );
         }
      }

      [TestMethod]
      public void AddTasksGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var sprintId = Sprints.ModelData.First().Id;
         var viewModel = ((ViewResult)_controller.AddTasks( sprintId )).Model as WorkItemsListForSprintViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void AddTasksGet_AddsCallingActionAndId_IfSpecified()
      {
         var sprintId = Sprints.ModelData.First().Id;
         var callingId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.AddTasks( sprintId, "Edit", callingId.ToString() )).Model as WorkItemsListForSprintViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( callingId, viewModel.CallingId );
      }
      #endregion


      #region AddTasks POST
      [TestMethod]
      public void AddTasksPost_AddsTasksToSprint()
      {
         RemoveAllWorkItemsFromSprints();

         var sprint = Sprints.ModelData.First( x => x.Project.Name == "Home Scrum" && !x.Status.TaskListIsClosed );

         var viewModel = CreateTasksForSprintViewModel( sprint );
         viewModel.WorkItems[1].IsInTargetSprint = true;
         viewModel.WorkItems[2].IsInTargetSprint = true;
         viewModel.WorkItems[4].IsInTargetSprint = true;

         _controller.AddTasks( viewModel );

         foreach (var item in viewModel.WorkItems)
         {
            var workItem = _session.Get<WorkItem>( item.Id );
            if (item.IsInTargetSprint)
            {
               Assert.AreEqual( sprint.Id, workItem.Sprint.Id );
            }
            else
            {
               Assert.IsNull( workItem.Sprint );
            }
         }
      }

      [TestMethod]
      public void AddTasksPost_RedirectsBackToDetails()
      {
         var sprint = Sprints.ModelData.First( x => x.Project.Name == "Home Scrum" && !x.Status.TaskListIsClosed );
         var viewModel = CreateTasksForSprintViewModel( sprint );

         var result = _controller.AddTasks( viewModel ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 2, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Details", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Assert.AreEqual( value, sprint.Id );
      }
      #endregion


      #region Private Helpers
      private static void AssertModelAndViewModelPropertiesEqual( Sprint model, SprintEditorViewModel viewModel )
      {
         Assert.AreEqual( model.Id, viewModel.Id );
         Assert.AreEqual( model.Name, viewModel.Name );
         Assert.AreEqual( model.Description, viewModel.Description );
         Assert.AreEqual( model.Goal, viewModel.Goal );
         Assert.AreEqual( model.StartDate.ToString(), viewModel.StartDate.ToString() );
         Assert.AreEqual( model.EndDate.ToString(), viewModel.EndDate.ToString() );
         Assert.AreEqual( model.Capacity, viewModel.Capacity );
      }

      private SprintEditorViewModel CreateSprintEditorViewModel()
      {
         return new SprintEditorViewModel()
         {
            Name = "New Work Item",
            Description = "This is a test",
            StatusId = SprintStatuses.ModelData.First( x => x.StatusCd == 'A' ).Id,
            StatusName = SprintStatuses.ModelData.First( x => x.StatusCd == 'A' ).Name,
            ProjectId = Projects.ModelData.First( x => x.Status.Category == ProjectStatusCategory.Active && x.Status.StatusCd == 'A' ).Id,
            ProjectName = Projects.ModelData.First( x => x.Status.Category == ProjectStatusCategory.Active && x.Status.StatusCd == 'A' ).Name,
            StartDate = new DateTime( 2013, 4, 1 ),
            EndDate = new DateTime( 2013, 4, 30 ),
            CreatedByUserId = Users.ModelData.First( x => x.StatusCd == 'A' ).Id,
            Capacity = 42
         };
      }

      private SprintEditorViewModel CreateSprintEditorViewModel( Sprint model )
      {
         return new SprintEditorViewModel()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            StatusId = model.Status.Id,
            StatusName = model.Status.Name,
            ProjectId = model.Project.Id,
            ProjectName = model.Project.Name,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            CreatedByUserId = model.CreatedByUser.Id,
            Capacity = model.Capacity
         };
      }

      private WorkItemsListForSprintViewModel CreateBacklogItemsForSprintViewModel( Sprint sprint )
      {
         var model = new WorkItemsListForSprintViewModel()
         {
            Id = sprint.Id,
            Name = sprint.Name
         };

         using (var tx = _session.BeginTransaction())
         {
            model.WorkItems = _session.Query<WorkItem>()
               .Where( x => x.Project.Id == sprint.Project.Id && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && (x.Sprint == null || x.Sprint.Id == sprint.Id) )
               .Select( x => new SprintWorkItemViewModel()
                             {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                StatusName = x.Status.Name,
                                WorkItemTypeName = x.WorkItemType.Name,
                                IsInTargetSprint = (x.Sprint != null)
                             } )
               .ToList();

            tx.Commit();
         }

         return model;
      }

      private WorkItemsListForSprintViewModel CreateTasksForSprintViewModel( Sprint sprint )
      {
         var model = new WorkItemsListForSprintViewModel()
         {
            Id = sprint.Id,
            Name = sprint.Name
         };

         using (var tx = _session.BeginTransaction())
         {
            model.WorkItems = _session.Query<WorkItem>()
               .Where( x => x.Project.Id == sprint.Project.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem && x.ParentWorkItem == null && (x.Sprint == null || x.Sprint.Id == sprint.Id) )
               .Select( x => new SprintWorkItemViewModel()
               {
                  Id = x.Id,
                  Name = x.Name,
                  Description = x.Description,
                  StatusName = x.Status.Name,
                  WorkItemTypeName = x.WorkItemType.Name,
                  IsInTargetSprint = (x.Sprint != null)
               } )
               .ToList();

            tx.Commit();
         }

         return model;
      }

      private void RemoveAllWorkItemsFromSprints()
      {
         using (var tx = _session.BeginTransaction())
         {
            var workItems = _session.Query<WorkItem>().Where( x => x.Sprint != null ).ToList();
            foreach (var workItem in workItems)
            {
               workItem.Sprint = null;
               _session.Update( workItem );
            }

            tx.Commit();
         }
      }
      #endregion
   }
}
