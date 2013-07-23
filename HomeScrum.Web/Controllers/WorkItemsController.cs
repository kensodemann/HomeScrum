using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Queries;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.WorkItems;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Ninject;
using Ninject.Extensions.Logging;

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
      public override System.Web.Mvc.ActionResult Index()
      {
         IEnumerable<WorkItemIndexViewModel> workItems;

         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            workItems = session.Query<WorkItem>()
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
         }

         return View( workItems );
      }


      #region Select Lists
      protected override void PopulateSelectLists( WorkItemEditorViewModel viewModel )
      {
         viewModel.Statuses = CreateSelectList<WorkItemStatus>( viewModel.StatusId );
         viewModel.WorkItemTypes = CreateWorkItemTypeSelectList( viewModel.WorkItemTypeId );
         viewModel.Projects = CreateProjectsSelectList( viewModel.ProjectId );
         viewModel.AssignedToUsers = CreateUserSelectList( viewModel.AssignedToUserId );
         viewModel.ProductBacklogItems = CreateProductBacklogSelectList( viewModel.ParentWorkItemId );
         base.PopulateSelectLists( viewModel );
      }

      private IEnumerable<SelectListItem> CreateSelectList<ModelT>( Guid selectedId )
         where ModelT : SystemDomainObject
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<ModelT>() { SelectedId = selectedId };
         using (var session = SessionFactory.OpenSession())
         {
            return query
               .GetLinqQuery( session )
               .SelectSelectListItems<ModelT>( selectedId );
         }
      }

      private IEnumerable<SelectListItemWithAttributes> CreateWorkItemTypeSelectList( Guid selectedId )
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<WorkItemType>() { SelectedId = selectedId };
         using (var session = SessionFactory.OpenSession())
         {
            return query
               .GetLinqQuery( session )
               .SelectSelectListItems( selectedId );
         }
      }

      private IEnumerable<SelectListItem> CreateProjectsSelectList( Guid selectedId )
      {
         using (var session = SessionFactory.OpenSession())
         {
            return session.Query<Project>()
               .Where( x => (x.Status.StatusCd == 'A' && x.Status.IsActive) || x.Id == selectedId )
               .OrderBy( x => x.Status.SortSequence )
               .ThenBy( x => x.Name.ToUpper() )
               .SelectSelectListItems<Project>( selectedId );
         }
      }

      private IEnumerable<SelectListItem> CreateUserSelectList( Guid selectedId )
      {
         using (var session = SessionFactory.OpenSession())
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
      }

      private IEnumerable<SelectListItemWithAttributes> CreateProductBacklogSelectList( Guid selectedId )
      {
         using (var session = SessionFactory.OpenSession())
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
      }
      #endregion


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
            .Where( x => x.UserName == userName )
            .ToList().First().Id;
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