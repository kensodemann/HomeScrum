﻿using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ReadWriteController<Project, ProjectViewModel, ProjectEditorViewModel>
   {
      public ProjectsController( IRepository<Project> repository, IRepository<ProjectStatus> projectStatusRepository, IValidator<Project> validator )
         : base( repository, validator )
      {
         _projectStatusRepository = projectStatusRepository;
      }

      private IRepository<ProjectStatus> _projectStatusRepository;

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
      public override ActionResult Create( ProjectEditorViewModel viewModel )
      {
         var model = Mapper.Map<Project>( viewModel );
         Validate( model, TransactionType.Insert );

         if (ModelState.IsValid)
         {
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
      public override ActionResult Edit( ProjectEditorViewModel viewModel )
      {
         var model = Mapper.Map<Project>( viewModel );
         Validate( model, TransactionType.Update );

         if (ModelState.IsValid)
         {
            MainRepository.Update( model );
            return RedirectToAction( () => this.Index() );
         }

         return View( viewModel );
      }
   }
}