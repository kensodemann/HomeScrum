using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.WorkItems;
using HomeScrum.Web.Translators;
using Ninject;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using HomeScrum.Data.SqlServer.Queries;
using HomeScrum.Data.SqlServer.Helpers;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadWriteController<WorkItem, WorkItemViewModel, WorkItemEditorViewModel>
   {
      [Inject]
      public WorkItemsController( IWorkItemRepository repository, IRepository<WorkItemStatus> statusRepository, IRepository<WorkItemType> workItemTypeRepository,
         IRepository<Project> projectRepository, IUserRepository userRepository, IValidator<WorkItem> validator, IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel> translator, ILogger logger )
         : base( repository, validator, translator, logger )
      {
         _statusRepository = statusRepository;
         _workItemTypeRepository = workItemTypeRepository;
         _projectRepository = projectRepository;
         _userRepository = userRepository;
         _workItemQuery = new WorkItemQuery();
      }

      private IRepository<WorkItemStatus> _statusRepository;
      private IRepository<WorkItemType> _workItemTypeRepository;
      private IRepository<Project> _projectRepository;
      private IUserRepository _userRepository;
      private WorkItemQuery _workItemQuery;

      protected override void PopulateSelectLists( WorkItemEditorViewModel viewModel )
      {
         viewModel.Statuses = _statusRepository.GetAll().ToSelectList( viewModel.StatusId );
         viewModel.WorkItemTypes = _workItemTypeRepository.GetAll().ToSelectList( viewModel.WorkItemTypeId );
         viewModel.Projects = _projectRepository.GetAll().ToSelectList( viewModel.ProjectId );
         viewModel.AssignedToUsers = _userRepository.GetAll().ToSelectList( allowUnassigned: true, selectedId: viewModel.AssignedToUserId );
         viewModel.ProductBacklogItems = ((IWorkItemRepository)MainRepository).GetOpenProductBacklog().ToSelectList( allowUnassigned: true, selectedId: viewModel.ParentWorkItemId );
         base.PopulateSelectLists( viewModel );
      }

      protected override void AddItem( WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.AddItem( model, user );
      }

      protected override void UpdateItem( WorkItem model, System.Security.Principal.IPrincipal user )
      {
         ClearNonAllowedItemsInModel( model );
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.UpdateItem( model, user );
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
         using (var session = NHibernateHelper.OpenSession())
         {
            var query = _workItemQuery.GetQuery( session );
            query.SetProjection(
               Projections.ProjectionList()
                  .Add( Projections.Property( "Id" ), "Id" )
                  .Add( Projections.Property( "Name" ), "Name" )
                  .Add( Projections.Property( "wit.Name" ), "WorkItemTypeName" )
                  .Add( Projections.Property( "stat.Name" ), "StatusName" )
                  .Add( Projections.Property( "stat.IsOpenStatus" ), "IsComplete" ) )
               .SetResultTransformer( Transformers.AliasToBean<WorkItemIndexViewModel>() );
            var workItems = query.List<WorkItemIndexViewModel>();

            return View( workItems );
         }
      }
   }
}