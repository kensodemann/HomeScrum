using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Admin;
using System;
using System.Security.Principal;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ReadWriteController<Project, ProjectViewModel, ProjectEditorViewModel>
   {
      public ProjectsController( IRepository<Project> repository, IRepository<ProjectStatus> projectStatusRepository, IUserRepository userRepository, IValidator<Project> validator )
         : base( repository, validator )
      {
         _projectStatusRepository = projectStatusRepository;
         _userRepository = userRepository;
      }

      private readonly IRepository<ProjectStatus> _projectStatusRepository;
      private readonly IUserRepository _userRepository;

      //
      // GET: /Projects/Create
      public override ActionResult Create()
      {
         var model = new ProjectEditorViewModel();

         model.ProjectStatuses = _projectStatusRepository.GetAll().ToSelectList();

         return View( model );
      }

      //
      // POST: /Projects/Create
      [HttpPost]
      public override ActionResult Create( ProjectEditorViewModel viewModel, IPrincipal user )
      {
         var model = Mapper.Map<Project>( viewModel );
         Validate( model, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
            MainRepository.Add( model );
            return RedirectToAction( () => this.Index() );
         }

         viewModel.ProjectStatuses = _projectStatusRepository.GetAll().ToSelectList();
         return View( viewModel );
      }

      //
      // GET: /Projects/Edit/Guid
      public override ActionResult Edit( Guid id )
      {
         var model = MainRepository.Get( id );

         if (model != null)
         {
            var viewModel = Mapper.Map<ProjectEditorViewModel>( model );
            viewModel.ProjectStatuses = _projectStatusRepository.GetAll().ToSelectList( model.ProjectStatus.Id );
            return View( viewModel );
         }

         return HttpNotFound();
      }

      //
      // POST: /Projects/Edit/Guid
      [HttpPost]
      public override ActionResult Edit( ProjectEditorViewModel viewModel, IPrincipal user )
      {
         var model = Mapper.Map<Project>( viewModel );
         Validate( model, TransactionType.Update );

         if (ModelState.IsValid)
         {
            model.LastModifiedUserRid = _userRepository.Get( user.Identity.Name ).Id;
            MainRepository.Update( model );
            return RedirectToAction( () => this.Index() );
         }

         return View( viewModel );
      }
   }
}