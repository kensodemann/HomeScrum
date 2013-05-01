using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.WorkItems;
using Ninject;
using System;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemsController : Base.ReadWriteController<WorkItem, WorkItemViewModel, WorkItemEditorViewModel>
   {
      [Inject]
      public WorkItemsController( IRepository<WorkItem> repository, IRepository<WorkItemStatus> statusRepository, IRepository<WorkItemType> workItemTypeRepository,
         IRepository<Project>projectRepository, IUserRepository userRepository, IValidator<WorkItem> validator )
         : base( repository, validator )
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

      //
      // Get: /WorkItems/Create
      public override ActionResult Create()
      {
         var viewModel = new WorkItemEditorViewModel();

         viewModel.Statuses = _statusRepository.GetAll().ToSelectList();
         viewModel.WorkItemTypes = _workItemTypeRepository.GetAll().ToSelectList();
         viewModel.Projects = _projectRepository.GetAll().ToSelectList();
         viewModel.Users = _userRepository.GetAll().ToSelectList( allowUnassigned: true );

         return View( viewModel );
      }
   }
}