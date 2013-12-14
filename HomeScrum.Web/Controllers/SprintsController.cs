using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class SprintsController : ReadWriteController<Sprint, SprintEditorViewModel>
   {
      public SprintsController( IPropertyNameTranslator<Sprint, SprintEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      //
      // GET: /Sprints/
      public override System.Web.Mvc.ActionResult Index()
      {
         Log.Debug( "Index()" );

         var session = SessionFactory.GetCurrentSession();
         var query = session.Query<Sprint>()
               .ApplyStandardSorting()
               .SelectSprintIndexViewModels();

         return IndexView( query );
      }


      //
      // GET: /Sprints/CurrentSprints
      public System.Web.Mvc.ActionResult CurrentSprints()
      {
         Log.Debug( "CurrentSprints()" );

         var session = SessionFactory.GetCurrentSession();
         var query = session.Query<Sprint>()
               .Where( x => x.Status.StatusCd == 'A' && x.Status.Category == SprintStatusCategory.Active &&
                            x.StartDate != null && x.StartDate <= DateTime.Now.Date && (x.EndDate == null || x.EndDate >= DateTime.Now.Date) )
               .ApplyStandardSorting()
               .SelectSprintIndexViewModels();

         return IndexView( query );
      }

      //
      // GET: /Sprints/OpenSprints
      public System.Web.Mvc.ActionResult OpenSprints()
      {
         Log.Debug( "CurrentSprints()" );

         var session = SessionFactory.GetCurrentSession();
         var query = session.Query<Sprint>()
               .Where( x => x.Status.StatusCd == 'A' && x.Status.Category != SprintStatusCategory.Complete )
               .ApplyStandardSorting()
               .SelectSprintIndexViewModels();

         return IndexView( query );
      }


      //
      // POST: /Sprints/Create
      public override ActionResult Create( SprintEditorViewModel viewModel, System.Security.Principal.IPrincipal user )
      {
         var session = SessionFactory.GetCurrentSession();
         viewModel.CreatedByUserId = user.Identity.GetUserId( session );
         return base.Create( viewModel, user );
      }


      //
      // GET: /Sprints/5/AddBacklogItems
      public virtual ActionResult AddBacklogItems( Guid id, string callingAction = null, string callingId = null )
      {
         var model = new WorkItemsListForSprintViewModel()
         {
            Id = id
         };

         UpdateNavigationStack( model, null, callingAction, callingId );

         var session = SessionFactory.GetCurrentSession();
         using (var tx = session.BeginTransaction())
         {
            var projectId = session.Query<Sprint>().Single( x => x.Id == id ).Project.Id;
            model.WorkItems = session.Query<WorkItem>()
               .Where( x => x.Status.Category != WorkItemStatusCategory.Complete && x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.Project.Id == projectId && (x.Sprint == null || x.Sprint.Id == id) )
               .ApplyStandardSorting()
               .SelectSprintWorkItemViewModel()
               .ToList();

            tx.Commit();
         }

         return View( model );
      }

      //
      // POST: /Sprints/5/AddBacklogItems
      [HttpPost]
      public virtual ActionResult AddBacklogItems( WorkItemsListForSprintViewModel viewModel )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var tx = session.BeginTransaction())
         {
            try
            {
               var sprint = session.Get<Sprint>( viewModel.Id );
               foreach (var item in viewModel.WorkItems)
               {
                  UpdateSprintOnWorkItem( session, item.Id, (item.IsInTargetSprint) ? sprint : null );
                  UpdateSprintOnChildTasks( session, item.Id, (item.IsInTargetSprint) ? sprint : null );
               }
               tx.Commit();
            }
            catch (Exception e)
            {
               tx.Rollback();
               Log.Error( e, "Error Processing Backlog Items" );
               ModelState.AddModelError( "Model", "An errror occurred processing this data.  Check the log files" );
               return View( viewModel );
            }
         }

         return RedirectToAction( "Edit", new { id = viewModel.Id } );
      }

      //
      // GET: /Sprints/5/AddTasks
      public virtual ActionResult AddTasks( Guid id, string callingAction = null, string callingId = null )
      {
         var model = new WorkItemsListForSprintViewModel()
         {
            Id = id
         };

         UpdateNavigationStack( model, null, callingAction, callingId );

         var session = SessionFactory.GetCurrentSession();
         using (var tx = session.BeginTransaction())
         {
            var projectId = session.Query<Sprint>().Single( x => x.Id == id ).Project.Id;
            model.WorkItems = session.Query<WorkItem>()
               .Where( x => x.Status.Category != WorkItemStatusCategory.Complete && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem && x.Project.Id == projectId && x.ParentWorkItemRid == null && (x.Sprint == null || x.Sprint.Id == id) )
               .ApplyStandardSorting()
               .SelectSprintWorkItemViewModel()
               .ToList();

            tx.Commit();
         }

         return View( model );
      }


      //
      // POST: /Sprints/5/AddBacklogItems
      [HttpPost]
      public virtual ActionResult AddTasks( WorkItemsListForSprintViewModel viewModel )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var tx = session.BeginTransaction())
         {
            try
            {
               var sprint = session.Get<Sprint>( viewModel.Id );
               foreach (var item in viewModel.WorkItems)
               {
                  UpdateSprintOnWorkItem( session, item.Id, (item.IsInTargetSprint) ? sprint : null );
               }
               tx.Commit();
            }
            catch (Exception e)
            {
               tx.Rollback();
               Log.Error( e, "Error Processing Backlog Items" );
               ModelState.AddModelError( "Model", "An errror occurred processing this data.  Check the log files" );
               return View( viewModel );
            }
         }

         return RedirectToAction( "Edit", new { id = viewModel.Id } );
      }

      private void UpdateSprintOnWorkItem( ISession session, Guid id, Sprint sprint )
      {
         Log.Debug( "UpdateSprintOnWorkItem( {0}, {1} )", id.ToString(), (sprint == null) ? "Null" : sprint.Name );
         var workItem = session.Get<WorkItem>( id );
         workItem.Sprint = sprint;
         session.Update( workItem );
      }


      private void UpdateSprintOnChildTasks( ISession session, Guid parentId, Sprint sprint )
      {
         Log.Debug( "UpdateSprintOnWorkItem( {0}, {1} )", parentId.ToString(), (sprint == null) ? "Null" : sprint.Name );
         var tasks = session.Query<WorkItem>().Where( x => x.ParentWorkItemRid == parentId ).ToList();
         foreach (var task in tasks)
         {
            task.Sprint = sprint;
            session.Update( task );
         }
      }


      protected override void PopulateSelectLists( ISession session, SprintEditorViewModel viewModel )
      {
         viewModel.Statuses = CreateSprintStatusSelectList( session, viewModel.StatusId );
         viewModel.Projects = CreateProjectsSelectList( session, viewModel.ProjectId );
         base.PopulateSelectLists( session, viewModel );
      }

      private IEnumerable<SelectListItemWithAttributes> CreateSprintStatusSelectList( ISession session, Guid selectedId )
      {
         return session.Query<SprintStatus>()
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

      protected override void Save( ISession session, Sprint model, System.Security.Principal.IPrincipal user )
      {
         model.LastModifiedUserRid = user.Identity.GetUserId( session );
         base.Save( session, model, user );
      }

      protected override void Update( ISession session, Sprint model, System.Security.Principal.IPrincipal user )
      {
         model.LastModifiedUserRid = user.Identity.GetUserId( session );
         base.Update( session, model, user );
      }

      protected override SprintEditorViewModel GetEditorViewModel( ISession session, Guid id )
      {
         var viewModel = base.GetEditorViewModel( session, id );

         if (viewModel != null)
         {
            viewModel.BacklogItems = GetBacklogItems( session, id );
            viewModel.Tasks = GetTasks( session, id );
            viewModel.TotalPoints = viewModel.Tasks.Sum( x => x.Points );
         }

         return viewModel;
      }

      private IEnumerable<SprintWorkItemViewModel> GetBacklogItems( ISession session, Guid id )
      {
         return session.Query<WorkItem>()
            .Where( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.Sprint.Id == id )
            .ApplyStandardSorting()
            .SelectSprintWorkItemViewModel()
            .ToList();
      }

      private IEnumerable<SprintWorkItemViewModel> GetTasks( ISession session, Guid id )
      {
         return session.Query<WorkItem>()
            .Where( x => x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem && x.Sprint.Id == id )
            .ApplyStandardSorting()
            .SelectSprintWorkItemViewModel()
            .ToList();
      }
   }

   internal static class SprintQueryExtentions
   {
      public static IQueryable<Sprint> ApplyStandardSorting( this IQueryable<Sprint> query )
      {
         return query.OrderBy( x => x.Project.Name )
            .ThenBy( x => x.StartDate ?? DateTime.MaxValue )
            .ThenBy( x => x.Status.SortSequence );
      }

      public static IQueryable<WorkItem> ApplyStandardSorting( this IQueryable<WorkItem> query )
      {
         return query.OrderBy( x => (x.Sprint == null) ? 1 : 2 )
            .ThenBy( x => x.WorkItemType.SortSequence )
            .ThenBy( x => x.Status.SortSequence )
            .ThenBy( x => x.Name );
      }

      public static IQueryable<SprintIndexViewModel> SelectSprintIndexViewModels( this IQueryable<Sprint> query )
      {
         return query.Select( x => new SprintIndexViewModel()
            {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description,
               ProjectName = x.Project.Name,
               StatusName = x.Status.Name,
               StartDate = x.StartDate,
               EndDate = x.EndDate
            } );
      }

      public static IQueryable<SprintWorkItemViewModel> SelectSprintWorkItemViewModel( this IQueryable<WorkItem> query )
      {
         return query.Select( x => new SprintWorkItemViewModel()
            {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description,
               WorkItemTypeName = x.WorkItemType.Name,
               StatusName = x.Status.Name,
               IsInTargetSprint = x.Sprint != null,
               Points = (x.WorkItemType.Category == 0 ? (x.Tasks.Count() == 0 ? 0 : x.Tasks.Sum( t => t.Points )) : x.Points),
               PointsRemaining = (x.WorkItemType.Category == 0 ? (x.Tasks.Count() == 0 ? 0 : x.Tasks.Sum( t => t.PointsRemaining )) : x.PointsRemaining)
            } );
      }
   }
}
