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
               .Select( x => new AvailableWorkItemsViewModel()
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
                  var workItem = session.Get<WorkItem>( item.Id );
                  workItem.Sprint = (item.IsInTargetSprint) ? sprint : null;
                  UpdateSprintOnTasks( item.Id, (item.IsInTargetSprint) ? sprint : null );
                  session.Update( workItem );
               }
               tx.Commit();
            }
            catch
            {
               tx.Rollback();
               // TODO: This should probably re-direct to some sort of error page...
               //       Also, log the exception.
            }
         }
         
         // TODO: this should redirect to the edit action.
         return RedirectToAction( () => this.Index() );
      }


      private void UpdateSprintOnTasks( Guid parentId, Sprint sprint )
      {
         var session = SessionFactory.GetCurrentSession();
         Debug.Assert( session.Transaction.IsActive );

         var tasks = session.Query<WorkItem>().Where( x => x.ParentWorkItem != null && x.ParentWorkItem.Id == parentId ).ToList();
         foreach(var task in tasks)
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

      // TODO: Make extention to IPrincipal, replace this here and in WorkItem
      private Guid GetUserId( ISession session, string userName )
      {
         return session.Query<User>()
            .Single( x => x.UserName == userName ).Id;
      }
   }
}
