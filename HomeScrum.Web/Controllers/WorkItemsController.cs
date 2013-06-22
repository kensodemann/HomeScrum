using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.WorkItems;
using HomeScrum.Web.Translators;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadWriteController<WorkItem, WorkItemViewModel, WorkItemEditorViewModel>
   {
      [Inject]
      public WorkItemsController( IRepository<WorkItem> repository, IRepository<WorkItemStatus> statusRepository, IRepository<WorkItemType> workItemTypeRepository,
         IRepository<Project> projectRepository, IUserRepository userRepository, IValidator<WorkItem> validator, IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel> translator )
         : base( repository, validator, translator )
      {
         _statusRepository = statusRepository;
         _workItemTypeRepository = workItemTypeRepository;
         _projectRepository = projectRepository;
         _userRepository = userRepository;
      }

      private IRepository<WorkItemStatus> _statusRepository;
      private IRepository<WorkItemType> _workItemTypeRepository;
      private IRepository<Project> _projectRepository;
      private IUserRepository _userRepository;

      protected override void PopulateSelectLists( WorkItemEditorViewModel viewModel )
      {
         viewModel.Statuses = _statusRepository.GetAll().ToSelectList( viewModel.StatusId );
         viewModel.WorkItemTypes = _workItemTypeRepository.GetAll().ToSelectList( viewModel.WorkItemTypeId );
         viewModel.Projects = _projectRepository.GetAll().ToSelectList( viewModel.ProjectId );
         viewModel.AssignedToUsers = _userRepository.GetAll().ToSelectList( allowUnassigned: true, selectedId: viewModel.AssignedToUserId );
         base.PopulateSelectLists( viewModel );
      }

      protected override void AddItem( WorkItem model, System.Security.Principal.IPrincipal user )
      {
         if (!model.WorkItemType.IsTask)
         {
            model.AssignedToUser = null;
         }
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.AddItem( model, user );
      }

      protected override void UpdateItem( WorkItem model, System.Security.Principal.IPrincipal user )
      {
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.UpdateItem( model, user );
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
      // TODO: Look at moving the default sort into the repository
      public override System.Web.Mvc.ActionResult Index()
      {
         var workItems = MainRepository
            .GetAll()
            .OrderBy( x => x.Status.SortSequence )
            .OrderBy( x => x.WorkItemType.SortSequence )
            .ToList();

         return View( Mapper.Map<ICollection<WorkItem>, IEnumerable<WorkItemViewModel>>( workItems ) );
      }
   }
}