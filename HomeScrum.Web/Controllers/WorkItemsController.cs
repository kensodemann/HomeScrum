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

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadWriteController<WorkItem, WorkItemViewModel, WorkItemEditorViewModel>
   {
      [Inject]
      public WorkItemsController( IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }

      //
      // POST: /WorkItem/Create
      public override ActionResult Create( WorkItemEditorViewModel viewModel, System.Security.Principal.IPrincipal user )
      {
         // The base Create() does a validation before calling AddItem().
         // This data must be set before the validation.
         var session = SessionFactory.GetCurrentSession();
         viewModel.CreatedByUserId = GetUserId( session, user.Identity.Name );
         return base.Create( viewModel, user );
      }

      //
      // GET: /WorkItems/
      public override ActionResult Index()
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var workItems = session.Query<WorkItem>()
               .OrderBy( x => x.WorkItemType.SortSequence )
               .ThenBy( x => x.Status.SortSequence )
               .ThenBy( x => x.Name.ToUpper() )
               .Select( x => new WorkItemIndexViewModel()
                             {
                                Id = x.Id,
                                Name = x.Name,
                                WorkItemTypeName = x.WorkItemType.Name,
                                StatusName = x.Status.Name,
                                IsComplete = !x.Status.IsOpenStatus
                             } )
               .ToList();

            transaction.Commit();
            return View( workItems );
         }
      }


      public ActionResult EditTask( Guid id, string callingAction, string callingId )
      {
         var result = Edit( id );

         if (result is ViewResult)
         {
            var viewModel = ((ViewResult)result).Model as WorkItemEditorViewModel;
            viewModel.CallingId = new Guid( callingId );
            viewModel.CallingAction = callingAction;
         }

         return result;
      }

      [HttpPost]
      public ActionResult EditTask( WorkItemEditorViewModel viewModel, System.Security.Principal.IPrincipal user )
      {
         var result = base.Edit( viewModel, user );

         if (result is RedirectToRouteResult && viewModel.CallingAction != null)
         {
            return RedirectToAction( viewModel.CallingAction, new { id = viewModel.CallingId.ToString() } );
         }

         return result;
      }


      #region Select Lists
      protected override void PopulateSelectLists( ISession session, WorkItemEditorViewModel viewModel )
      {
         viewModel.Statuses = CreateSelectList<WorkItemStatus>( session, viewModel.StatusId );
         viewModel.WorkItemTypes = CreateWorkItemTypeSelectList( session, viewModel.WorkItemTypeId );
         viewModel.Projects = CreateProjectsSelectList( session, viewModel.ProjectId );
         viewModel.AssignedToUsers = CreateUserSelectList( session, viewModel.AssignedToUserId );
         viewModel.ProductBacklogItems = CreateProductBacklogSelectList( session, viewModel.ParentWorkItemId );
         base.PopulateSelectLists( session, viewModel );
      }

      private IEnumerable<SelectListItem> CreateSelectList<ModelT>( ISession session, Guid selectedId )
         where ModelT : SystemDomainObject
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<ModelT>() { SelectedId = selectedId };

         return query
            .GetQuery( session )
            .SelectSelectListItems<ModelT>( selectedId );
      }

      private IEnumerable<SelectListItemWithAttributes> CreateWorkItemTypeSelectList( ISession session, Guid selectedId )
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<WorkItemType>() { SelectedId = selectedId };

         return query
            .GetQuery( session )
            .SelectSelectListItems( selectedId );
      }

      private IEnumerable<SelectListItem> CreateProjectsSelectList( ISession session, Guid selectedId )
      {
         return session.Query<Project>()
             .Where( x => (x.Status.StatusCd == 'A' && x.Status.IsActive) || x.Id == selectedId )
             .OrderBy( x => x.Status.SortSequence )
             .ThenBy( x => x.Name.ToUpper() )
             .SelectSelectListItems<Project>( selectedId );
      }

      private IEnumerable<SelectListItem> CreateUserSelectList( ISession session, Guid selectedId )
      {
         var users = session.Query<User>()
            .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
            .OrderBy( x => x.LastName.ToUpper() )
            .ThenBy( x => x.FirstName.ToUpper() )
            .SelectSelectListItems( selectedId );

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
              .Where( x => (x.Status.StatusCd == 'A' && x.Status.IsOpenStatus &&
                            x.WorkItemType.StatusCd == 'A' && !x.WorkItemType.IsTask) || x.Id == selectedId )
              .OrderBy( x => x.WorkItemType.SortSequence )
              .ThenBy( x => x.Status.SortSequence )
              .ThenBy( x => x.Name.ToUpper() )
              .SelectSelectListItems( selectedId );

         backlog.Insert( 0, new SelectListItemWithAttributes()
                            {
                               Value = default( Guid ).ToString(),
                               Text = DisplayStrings.NotAssigned,
                               Selected = (selectedId == default( Guid )),
                               DataAttributes = new Dictionary<string, string>()
                                                   {
                                                      { "ProjectId", default( Guid ).ToString() }
                                                   }
                            } );
         return backlog;
      }
      #endregion


      protected override WorkItemEditorViewModel GetViewModel( ISession session, Guid id )
      {
         var viewModel = base.GetViewModel( session, id );

         if (viewModel != null)
         {
            viewModel.Tasks =
               session.Query<WorkItem>()
                  .Where( x => x.ParentWorkItem.Id == id )
                  .OrderBy( x => x.Status.SortSequence )
                  .ThenBy( x => x.WorkItemType.SortSequence )
                  .ThenBy( x => x.Name.ToUpper() )
                  .Select( x => new WorkItemIndexViewModel()
                                {
                                   Id = x.Id,
                                   Name = x.Name,
                                   Description = x.Description,
                                   StatusName = x.Status.Name,
                                   WorkItemTypeName = x.WorkItemType.Name,
                                   IsComplete = !x.Status.IsOpenStatus
                                } )
                  .ToList();
         }

         return viewModel;
      }


      protected override void Save( ISession session, WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = GetUserId( session, user.Identity.Name );
         base.Save( session, model, user );
      }

      protected override void Update( ISession session, WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = GetUserId( session, user.Identity.Name );
         base.Update( session, model, user );
      }

      private Guid GetUserId( ISession session, string userName )
      {
         return session.Query<User>()
            .Single( x => x.UserName == userName ).Id;
      }

      private void ClearNonAllowedItemsInModel( WorkItem model )
      {
         if (!model.WorkItemType.IsTask)
         {
            model.AssignedToUser = null;
            model.ParentWorkItem = null;
         }
      }
   }
}