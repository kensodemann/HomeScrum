using System;
using System.Web.Mvc;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer.Queries;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.WorkItems;
using HomeScrum.Web.Translators;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Ninject;
using Ninject.Extensions.Logging;
using NHibernate.Linq;
using System.Linq;
using System.Collections.Generic;
using HomeScrum.Web.Extensions;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadWriteController<WorkItem, WorkItemViewModel, WorkItemEditorViewModel>
   {
      [Inject]
      public WorkItemsController( IWorkItemRepository repository,
         IRepository<Project> projectRepository, IUserRepository userRepository, IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory )
      {
         _projectRepository = projectRepository;
         _userRepository = userRepository;
         _workItemQuery = new WorkItemQuery();
         _sessionFactory = sessionFactory;
         _workItemRepository = repository;
      }

      private IRepository<Project> _projectRepository;
      private IUserRepository _userRepository;
      private WorkItemQuery _workItemQuery;
      private ISessionFactory _sessionFactory;
      private IWorkItemRepository _workItemRepository;

      protected override void PopulateSelectLists( WorkItemEditorViewModel viewModel )
      {
         viewModel.Statuses = CreateSelectList<WorkItemStatus>( viewModel.StatusId );
         viewModel.WorkItemTypes = CreateWorkItemTypeSelectList( viewModel.WorkItemTypeId );
         viewModel.Projects = _projectRepository.GetAll().ToSelectList( viewModel.ProjectId );
         viewModel.AssignedToUsers = _userRepository.GetAll().ToSelectList( allowUnassigned: true, selectedId: viewModel.AssignedToUserId );
         viewModel.ProductBacklogItems = _workItemRepository.GetOpenProductBacklog().ToSelectList( allowUnassigned: true, selectedId: viewModel.ParentWorkItemId );
         base.PopulateSelectLists( viewModel );
      }

      private IEnumerable<SelectListItem> CreateSelectList<ModelT>( Guid selectedId )
         where ModelT : SystemDomainObject
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<ModelT>(){SelectedId = selectedId};
         using (var session = NHibernateHelper.OpenSession())
         {
            return query
               .GetLinqQuery( session )
               .SelectSelectListItems<ModelT>( selectedId );
         }
      }

      private IEnumerable<SelectListItemWithAttributes> CreateWorkItemTypeSelectList( Guid selectedId )
      {
         var query = new HomeScrum.Data.Queries.ActiveSystemObjectsOrdered<WorkItemType>() { SelectedId = selectedId };
         using (var session = NHibernateHelper.OpenSession())
         {
            return query
               .GetLinqQuery( session )
               .SelectSelectListItems( selectedId );
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
         viewModel.CreatedByUserId = _userRepository.Get( user.Identity.Name ).Id;
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