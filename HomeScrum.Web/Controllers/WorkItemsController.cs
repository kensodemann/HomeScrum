using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.WorkItems;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Linq;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using HomeScrum.Data.Services;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadWriteController<WorkItem, WorkItemEditorViewModel>
   {
      private readonly ISprintCalendarService _sprintCalendarService;

      [Inject]
      public WorkItemsController( IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory, ISprintCalendarService sprintCalendarService )
         : base( translator, logger, sessionFactory )
      {
         _sprintCalendarService = sprintCalendarService;
      }

      //
      // GET: /WorkItems/
      public override ActionResult Index()
      {
         var session = SessionFactory.GetCurrentSession();
         var query = BaseWorkItemQuery( session )
               .SelectWorkItemIndexViewModels();

         return IndexView( query );
      }

      //
      // GET: /WorkItems/MyAssignments
      public ActionResult MyAssignments( System.Security.Principal.IPrincipal user )
      {
         var session = SessionFactory.GetCurrentSession();
         var assignedToUserId = user.Identity.GetUserId( session );
         var query = BaseWorkItemQuery( session )
               .Where( x => x.AssignedToUser != null && x.AssignedToUser.Id == assignedToUserId && x.Status.Category != WorkItemStatusCategory.Complete )
               .SelectWorkItemIndexViewModels();

         return IndexView( query );
      }

      //
      // GET: /WorkItems/UnassignedBacklog
      public ActionResult UnassignedBacklog()
      {
         var session = SessionFactory.GetCurrentSession();
         var query = BaseWorkItemQuery( session )
              .Where( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem &&
                 x.Status.Category != WorkItemStatusCategory.Complete &&
                 x.Sprint == null )
              .SelectWorkItemIndexViewModels();

         return IndexView( query );
      }

      //
      // GET: /WorkItems/UnassignedProblems
      public ActionResult UnassignedProblems()
      {
         var session = SessionFactory.GetCurrentSession();
         var query = BaseWorkItemQuery( session )
               .Where( x => x.AssignedToUser == null
                  && x.Status.Category != WorkItemStatusCategory.Complete
                  && x.WorkItemType.Category == WorkItemTypeCategory.Issue )
               .SelectWorkItemIndexViewModels();

         return IndexView( query );
      }

      //
      // GET: /WorkItems/UnassignedTasks
      public ActionResult UnassignedTasks()
      {
         var session = SessionFactory.GetCurrentSession();
         var query = BaseWorkItemQuery( session )
               .Where( x => x.AssignedToUser == null
                  && x.Status.Category != WorkItemStatusCategory.Complete
                  && x.WorkItemType.Category == WorkItemTypeCategory.Task )
               .SelectWorkItemIndexViewModels();

         return IndexView( query );
      }

      private IQueryable<WorkItem> BaseWorkItemQuery( ISession session )
      {
         return session.Query<WorkItem>()
            .OrderBy( x => x.WorkItemType.SortSequence )
            .ThenBy( x => x.Status.SortSequence )
            .ThenBy( x => x.Name.ToUpper() );
      }

      //
      // GET: /WorkItem/Create
      public override ActionResult Create( string callingController = null, string callingAction = null, string callingId = null, string parentId = null )
      {
         Guid parsedId;

         var view = base.Create( callingController, callingAction, callingId, parentId ) as ViewResult;
         var model = (WorkItemEditorViewModel)view.Model;

         if (Guid.TryParse( parentId, out parsedId ))
         {
            model.ClearSelectedBacklog();
            model.ClearSelectedSprint();

            var backlogItem = model.ProductBacklogItems.FirstOrDefault( x => new Guid( x.Value ) == parsedId );
            if (backlogItem != null)
            {
               backlogItem.Selected = true;
               model.ParentWorkItemId = parsedId;

               var sprint = model.Sprints.SingleOrDefault( x => x.Value == backlogItem.DataAttributes["SprintId"] );
               if (sprint != null)
               {
                  sprint.Selected = true;
                  model.SprintId = new Guid( sprint.Value );
               }
            }
         }

         return view;
      }


      //
      // POST: /WorkItem/Create
      public override ActionResult Create( WorkItemEditorViewModel viewModel, System.Security.Principal.IPrincipal user )
      {
         // The base Create() does a validation before calling AddItem().
         // This data must be set before the validation.
         var session = SessionFactory.GetCurrentSession();
         viewModel.CreatedByUserId = user.Identity.GetUserId( session );
         return base.Create( viewModel, user );
      }

      //
      // GET: /WorkItems/RemoveParent/Id
      public virtual ActionResult RemoveParent( Guid id, string callingAction = null, string callingId = null )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               var workItem = session.Get<WorkItem>( id );
               if (workItem != null)
               {
                  workItem.ParentWorkItem = null;
                  session.Save( workItem );
               }
            }
            catch (Exception e)
            {
               transaction.Rollback();
               Log.Error( e, "Attempting to remove parent" );
            }
            finally
            {
               transaction.Commit();
            }
         }

         return RedirectToAction( callingAction ?? "Index",
            callingId == null ? null : new { id = callingId.ToString() } );
      }


      #region Select Lists
      protected override void PopulateSelectLists( ISession session, WorkItemEditorViewModel viewModel )
      {
         viewModel.Statuses = CreateStatusSelectList( session, viewModel.StatusId );
         viewModel.WorkItemTypes = CreateWorkItemTypeSelectList( session, viewModel.WorkItemTypeId );
         viewModel.Projects = CreateProjectsSelectList( session, viewModel.ProjectId );
         viewModel.AssignedToUsers = CreateUserSelectList( session, viewModel.AssignedToUserId );
         viewModel.ProductBacklogItems = CreateProductBacklogSelectList( session, viewModel.ParentWorkItemId );
         viewModel.Sprints = CreateSprintSelectList( session, viewModel.SprintId );
         base.PopulateSelectLists( session, viewModel );
      }

      private IEnumerable<SelectListItemWithAttributes> CreateStatusSelectList( ISession session, Guid selectedId )
      {
         return session.Query<WorkItemStatus>()
            .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
            .OrderBy( x => x.SortSequence )
            .SelectSelectListItems( selectedId )
            .ToList();
      }

      private IEnumerable<SelectListItemWithAttributes> CreateWorkItemTypeSelectList( ISession session, Guid selectedId )
      {
         return session.Query<WorkItemType>()
            .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
            .OrderBy( x => x.SortSequence )
            .SelectSelectListItems( selectedId )
            .ToList();
      }

      private IEnumerable<SelectListItem> CreateProjectsSelectList( ISession session, Guid selectedId )
      {
         return session.Query<Project>()
             .Where( x => (x.Status.StatusCd == 'A' && x.Status.Category == ProjectStatusCategory.Active) || x.Id == selectedId )
             .OrderBy( x => x.Status.SortSequence )
             .ThenBy( x => x.Name.ToUpper() )
             .SelectSelectListItems<Project>( selectedId )
             .ToList();
      }

      private IEnumerable<SelectListItem> CreateUserSelectList( ISession session, Guid selectedId )
      {
         var users = session.Query<User>()
            .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
            .OrderBy( x => x.LastName.ToUpper() )
            .ThenBy( x => x.FirstName.ToUpper() )
            .SelectSelectListItems( selectedId )
            .ToList();

         users.Insert( 0, new SelectListItem()
                          {
                             Value = default( Guid ).ToString(),
                             Text = DisplayStrings.NotAssigned,
                             Selected = (selectedId == default( Guid ))
                          } );

         return users;
      }

      private IEnumerable<SelectListItemWithAttributes> CreateProductBacklogSelectList( ISession session, Guid selectedId )
      {
         var backlog = session.Query<WorkItem>()
              .Where( x => (x.Status.StatusCd == 'A' && x.Status.Category != WorkItemStatusCategory.Complete &&
                            x.WorkItemType.StatusCd == 'A' && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem) || x.Id == selectedId )
              .OrderBy( x => x.WorkItemType.SortSequence )
              .ThenBy( x => x.Status.SortSequence )
              .ThenBy( x => x.Name.ToUpper() )
              .SelectSelectListItems( selectedId )
              .ToList();

         backlog.Insert( 0, new SelectListItemWithAttributes()
                            {
                               Value = default( Guid ).ToString(),
                               Text = DisplayStrings.NotAssigned,
                               Selected = (selectedId == default( Guid )),
                               DataAttributes = new Dictionary<string, string>()
                                                    {
                                                       { "ProjectId", default( Guid ).ToString() },
                                                       { "SprintId", default( Guid ).ToString() }
                                                    }
                            } );
         return backlog;
      }

      private IEnumerable<SelectListItemWithAttributes> CreateSprintSelectList( ISession session, Guid selectedId )
      {
         var sprints = session.Query<Sprint>()
            .Where( x => (x.Status.StatusCd == 'A' && x.Status.Category != SprintStatusCategory.Complete &&
                         (!x.Status.BacklogIsClosed || !x.Status.TaskListIsClosed)) || x.Id == selectedId )
            .OrderBy( x => x.Status.SortSequence )
            .ThenBy( x => (x.StartDate ?? DateTime.MaxValue) )
            .ThenBy( x => x.Name.ToUpper() )
            .SelectSelectListItems( selectedId )
            .ToList();

         sprints.Insert( 0, new SelectListItemWithAttributes()
                            {
                               Value = default( Guid ).ToString(),
                               Text = DisplayStrings.NotAssigned,
                               Selected = (selectedId == Guid.Empty),
                               DataAttributes = new Dictionary<string, string>()
                                                {
                                                   { "ProjectId", Guid.Empty.ToString() },
                                                   { "TaskListIsClosed", "False" },
                                                   { "BacklogIsClosed", "False" }
                                                }
                            } );

         return sprints;
      }
      #endregion


      protected override WorkItemEditorViewModel GetEditorViewModel( ISession session, Guid id )
      {
         var viewModel = base.GetEditorViewModel( session, id );

         if (viewModel != null)
         {
            viewModel.Tasks = GetChildTasks( session, id );
            if (viewModel.Tasks.Count()> 0)
            {
               viewModel.Points = viewModel.Tasks.Sum( x => x.Points );
               viewModel.PointsRemaining = viewModel.Tasks.Sum( x => x.PointsRemaining );
            }
         }

         return viewModel;
      }

      private IEnumerable<WorkItemIndexViewModel> GetChildTasks( ISession session, Guid id )
      {
         return session.Query<WorkItem>()
            .Where( x => x.ParentWorkItem.Id == id )
            .OrderBy( x => x.Status.SortSequence )
            .ThenBy( x => x.WorkItemType.SortSequence )
            .ThenBy( x => x.Name.ToUpper() )
            .SelectWorkItemIndexViewModels()
            .ToList();
      }


      protected override void Save( ISession session, WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = user.Identity.GetUserId( session );
         base.Save( session, model, user );

         UpdateSprintCalendar( session, model );
      }

      protected override void Update( ISession session, WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = user.Identity.GetUserId( session );
         base.Update( session, model, user );

         UpdateChildTasks( session, model );

         UpdateSprintCalendar( session, model );
      }

      private void UpdateSprintCalendar( ISession session, WorkItem model )
      {
         if (model.Sprint != null)
         {
            session.Flush();
            session.Refresh( model );
            _sprintCalendarService.Update( model.Sprint );
         }
      }

      private void UpdateChildTasks( ISession session, WorkItem model )
      {
         var children = session.Query<WorkItem>()
            .Where( x => x.ParentWorkItem != null && x.ParentWorkItem.Id == model.Id )
            .Cacheable()
            .ToList();

         foreach (var child in children)
         {
            PropagateChangesToChild( session, model, child );
         }
      }

      private static void PropagateChangesToChild( ISession session, WorkItem model, WorkItem child )
      {
         if (child.Project != model.Project || child.Sprint != model.Sprint)
         {
            child.LastModifiedUserRid = model.LastModifiedUserRid;
            child.Project = model.Project;
            child.Sprint = model.Sprint;
            session.Update( child );
         }
      }

      private void ClearNonAllowedItemsInModel( WorkItem model )
      {
         if (model.WorkItemType.Category == WorkItemTypeCategory.BacklogItem)
         {
            model.AssignedToUser = null;
            model.ParentWorkItem = null;
            model.Points = 0;
            model.PointsRemaining = 0;
         }
      }
   }
}