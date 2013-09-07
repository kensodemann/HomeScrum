using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeScrum.Web.Extensions;
using System.Diagnostics;

namespace HomeScrum.Web.Controllers
{
   public class SprintsController : ReadWriteController<Sprint, SprintViewModel, SprintEditorViewModel>
   {
      public SprintsController( IPropertyNameTranslator<Sprint, SprintEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      //
      // GET: /Sprints/
      public override System.Web.Mvc.ActionResult Index()
      {
         IEnumerable<SprintIndexViewModel> items;
         Log.Debug( "Index()" );

         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var queryModel = new HomeScrum.Data.Queries.AllDomainObjects<Sprint>();
            items = queryModel.GetQuery( session )
               .OrderBy( x => x.Project.Name )
               .ThenBy( x => x.StartDate ?? DateTime.MaxValue )
               .ThenBy( x => x.Status.SortSequence )
               .Select( x => new SprintIndexViewModel()
                             {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                ProjectName = x.Project.Name,
                                StatusName = x.Status.Name,
                                StartDate = x.StartDate,
                                EndDate = x.EndDate
                             } );

            transaction.Commit();
         }

         return View( items );
      }


      //
      // GET: /Sprints/CurrentSprints
      public System.Web.Mvc.ActionResult CurrentSprints()
      {
         IEnumerable<SprintIndexViewModel> items;
         Log.Debug( "CurrentSprints()" );

         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var queryModel = new HomeScrum.Data.Queries.AllDomainObjects<Sprint>();
            items = queryModel.GetQuery( session )
               .Where( x => x.Status.StatusCd == 'A' && x.Status.IsOpenStatus &&
                            x.StartDate != null && x.StartDate <= DateTime.Now.Date && (x.EndDate == null || x.EndDate >= DateTime.Now.Date) )
               .OrderBy( x => x.Project.Name )
               .ThenBy( x => x.StartDate )
               .ThenBy( x => x.Status.SortSequence )
               .Select( x => new SprintIndexViewModel()
               {
                  Id = x.Id,
                  Name = x.Name,
                  Description = x.Description,
                  ProjectName = x.Project.Name,
                  StatusName = x.Status.Name,
                  StartDate = x.StartDate,
                  EndDate = x.EndDate
               } );

            transaction.Commit();
         }

         return View( items );
      }


      //
      // POST: /Sprints/Create
      public override ActionResult Create( SprintEditorViewModel viewModel, System.Security.Principal.IPrincipal user )
      {
         var session = SessionFactory.GetCurrentSession();
         viewModel.CreatedByUserId = GetUserId( session, user.Identity.Name );
         return base.Create( viewModel, user );
      }


      //
      // GET: /Sprints/5/AddBacklogItems
      public virtual ActionResult AddBacklogItems( Guid id )
      {
         var model = new WorkItemsListForSprintViewModel()
         {
            Id = id
         };

         var session = SessionFactory.GetCurrentSession();
         using (var tx = session.BeginTransaction())
         {
            var projectId = session.Query<Sprint>().Single( x => x.Id == id ).Project.Id;
            model.WorkItems = session.Query<WorkItem>()
               .Where( x => x.Status.IsOpenStatus && !x.WorkItemType.IsTask && x.Project.Id == projectId && (x.Sprint == null || x.Sprint.Id == id) )
               .OrderBy( x => (x.Sprint == null) ? 1 : 2 )
               .ThenBy( x => x.WorkItemType.SortSequence )
               .ThenBy( x => x.Status.SortSequence )
               .ThenBy( x => x.Name )
               .Select( x => new SprintWorkItemViewModel()
                             {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                WorkItemTypeName = x.WorkItemType.Name,
                                StatusName = x.Status.Name,
                                IsInTargetSprint = x.Sprint != null
                             } ).ToList();

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
      public virtual ActionResult AddTasks( Guid id )
      {
         var model = new WorkItemsListForSprintViewModel()
         {
            Id = id
         };

         var session = SessionFactory.GetCurrentSession();
         using (var tx = session.BeginTransaction())
         {
            var projectId = session.Query<Sprint>().Single( x => x.Id == id ).Project.Id;
            model.WorkItems = session.Query<WorkItem>()
               .Where( x => x.Status.IsOpenStatus && x.WorkItemType.IsTask && x.Project.Id == projectId && x.ParentWorkItem == null && (x.Sprint == null || x.Sprint.Id == id) )
               .OrderBy( x => (x.Sprint == null) ? 1 : 2 )
               .ThenBy( x => x.WorkItemType.SortSequence )
               .ThenBy( x => x.Status.SortSequence )
               .ThenBy( x => x.Name )
               .Select( x => new SprintWorkItemViewModel()
               {
                  Id = x.Id,
                  Name = x.Name,
                  Description = x.Description,
                  WorkItemTypeName = x.WorkItemType.Name,
                  StatusName = x.Status.Name,
                  IsInTargetSprint = x.Sprint != null
               } ).ToList();

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
         var tasks = session.Query<WorkItem>().Where( x => x.ParentWorkItem != null && x.ParentWorkItem.Id == parentId ).ToList();
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

      private IEnumerable<SelectListItem> CreateSprintStatusSelectList( ISession session, Guid selectedId )
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<SprintStatus>() { SelectedId = selectedId };

         return query
            .GetQuery( session )
            .SelectSelectListItems<SprintStatus>( selectedId );
      }

      private IEnumerable<SelectListItem> CreateProjectsSelectList( ISession session, Guid selectedId )
      {
         return session.Query<Project>()
             .Where( x => (x.Status.StatusCd == 'A' && x.Status.IsActive) || x.Id == selectedId )
             .OrderBy( x => x.Status.SortSequence )
             .ThenBy( x => x.Name.ToUpper() )
             .SelectSelectListItems<Project>( selectedId );
      }

      protected override void Save( ISession session, Sprint model, System.Security.Principal.IPrincipal user )
      {
         model.LastModifiedUserRid = GetUserId( session, user.Identity.Name );
         base.Save( session, model, user );
      }

      protected override void Update( ISession session, Sprint model, System.Security.Principal.IPrincipal user )
      {
         model.LastModifiedUserRid = GetUserId( session, user.Identity.Name );
         base.Update( session, model, user );
      }

      protected override SprintEditorViewModel GetEditorViewModel( ISession session, Guid id )
      {
         var viewModel = base.GetEditorViewModel( session, id );

         if (viewModel != null)
         {
            viewModel.BacklogItems = GetBacklogItems( session, id );
            viewModel.Tasks = GetTasks( session, id );
         }

         return viewModel;
      }

      private IEnumerable<SprintWorkItemViewModel> GetBacklogItems( ISession session, Guid id )
      {
         return session.Query<WorkItem>()
            .Where( x => !x.WorkItemType.IsTask && x.Sprint.Id == id )
            .OrderBy( x => x.WorkItemType.SortSequence )
            .ThenBy( x => x.Status.SortSequence )
            .ThenBy( x => x.Name )
            .Select( x => new SprintWorkItemViewModel()
                          {
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description,
                             StatusName = x.Status.Name,
                             WorkItemTypeName = x.WorkItemType.Name,
                             IsInTargetSprint = true
                          } )
            .ToList();
      }

      private IEnumerable<SprintWorkItemViewModel> GetTasks( ISession session, Guid id )
      {
         return session.Query<WorkItem>()
            .Where( x => x.WorkItemType.IsTask && x.Sprint.Id == id )
            .OrderBy( x => x.WorkItemType.SortSequence )
            .ThenBy( x => x.Status.SortSequence )
            .ThenBy( x => x.Name )
            .Select( x => new SprintWorkItemViewModel()
            {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description,
               StatusName = x.Status.Name,
               WorkItemTypeName = x.WorkItemType.Name,
               IsInTargetSprint = true
            } )
            .ToList();
      }

      // TODO: Make extention to IPrincipal, replace this here and in WorkItem
      private Guid GetUserId( ISession session, string userName )
      {
         return session.Query<User>()
            .Single( x => x.UserName == userName ).Id;
      }
   }
}
