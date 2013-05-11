﻿using HomeScrum.Data.Domain;
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
         IRepository<Project> projectRepository, IUserRepository userRepository, IValidator<WorkItem> validator )
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
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.AddItem( model, user );
      }

      protected override void UpdateItem( WorkItem model, System.Security.Principal.IPrincipal user )
      {
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.UpdateItem( model, user );
      }

      public override ActionResult Create( WorkItemEditorViewModel viewModel, System.Security.Principal.IPrincipal user )
      {
         viewModel.CreatedByUserId = _userRepository.Get( user.Identity.Name ).Id;
         return base.Create( viewModel, user );
      }
   }
}