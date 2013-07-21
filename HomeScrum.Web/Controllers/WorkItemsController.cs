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
         : base( translator, logger, sessionFactory )
      {
         _workItemQuery = new WorkItemQuery();
         _sessionFactory = sessionFactory;
      }

      private WorkItemQuery _workItemQuery;
      private ISessionFactory _sessionFactory;

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
         using (var session = _sessionFactory.OpenSession())
         {
            return query
               .GetLinqQuery( session )
               .SelectSelectListItems<ModelT>( selectedId );
         }
      }

      private IEnumerable<SelectListItemWithAttributes> CreateWorkItemTypeSelectList( Guid selectedId )
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<WorkItemType>() { SelectedId = selectedId };
         using (var session = _sessionFactory.OpenSession())
         {
            return query
               .GetLinqQuery( session )
               .SelectSelectListItems( selectedId );
         }
      }

      private IEnumerable<SelectListItem> CreateProjectsSelectList( Guid selectedId )
      {
         using (var session = _sessionFactory.OpenSession())
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
         using (var session = _sessionFactory.OpenSession())
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
         using (var session = _sessionFactory.OpenSession())
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

      protected override void AddItem( WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = GetUserId( user.Identity.Name );
         base.AddItem( model, user );
      }

      protected override void UpdateItem( WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = GetUserId( user.Identity.Name );
         base.UpdateItem( model, user );
      }

      private Guid GetUserId( string userName )
      {
         using (var session = NHibernateHelper.OpenSession())
         {
            return session.Query<User>()
               .Where( x => x.UserName == userName )
               .ToList().First().Id;
         }
      }


      private void ClearNonAllowedItemsInModel( WorkItem model )
      {
         if (!model.WorkItemType.IsTask)
         {
            model.AssignedToUser = null;
         }
      }

      //
      // POST: /WorkItem/Create
      public override ActionResult Create( WorkItemEditorViewModel viewModel, System.Security.Principal.IPrincipal user )
      {
         // The base Create() does a validation before calling AddItem().
         // This data must be set before the validation.
         viewModel.CreatedByUserId = GetUserId( user.Identity.Name );
         return base.Create( viewModel, user );
      }

      //
      // GET: /WorkItems/
      public override System.Web.Mvc.ActionResult Index()
      {
         using (var session = _sessionFactory.OpenSession())
         {
            var query = _workItemQuery.GetQuery( session );
            query.SetProjection(
               Projections.ProjectionList()
                  .Add( Projections.Property( "Id" ), "Id" )
                  .Add( Projections.Property( "Name" ), "Name" )
                  .Add( Projections.Property( "wit.Name" ), "WorkItemTypeName" )
                  .Add( Projections.Property( "stat.Name" ), "StatusName" )
                  .Add( Projections.Property( "stat.IsOpenStatus" ), "IsOpenStatus" ) )  // find better way to get IsCompleted
               .SetResultTransformer( Transformers.AliasToBean<WorkItemIndexViewModel>() );
            var workItems = query.List<WorkItemIndexViewModel>();

            return View( workItems );
         }
      }
   }
}