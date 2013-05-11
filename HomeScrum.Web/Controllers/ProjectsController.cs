using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using System;
using System.Security.Principal;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ReadWriteController<Project, ProjectViewModel, ProjectEditorViewModel>
   {
      public ProjectsController( IRepository<Project> repository, IRepository<ProjectStatus> projectStatusRepository, IUserRepository userRepository,
         IValidator<Project> validator, IPropertyNameTranslator<ProjectEditorViewModel> translator )
         : base( repository, validator, translator )
      {
         _projectStatusRepository = projectStatusRepository;
         _userRepository = userRepository;
      }

      private readonly IRepository<ProjectStatus> _projectStatusRepository;
      private readonly IUserRepository _userRepository;

      protected override void PopulateSelectLists( ProjectEditorViewModel viewModel )
      {
         base.PopulateSelectLists( viewModel );
         viewModel.Statuses = _projectStatusRepository.GetAll().ToSelectList( viewModel.StatusId );
      }


      protected override void AddItem( Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.AddItem( model, user );
      }

      protected override void UpdateItem( Project model, IPrincipal user )
      {
         model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
         base.UpdateItem( model, user );
      }
   }
}