﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System.Web.Mvc;
using HomeScrum.Web.Extensions;

namespace HomeScrum.Web.Controllers
{
   public class ProjectsController : ValidatingController<Project>
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

      ////
      //// POST: /Projects/Create
      //[HttpPost]
      //public virtual ActionResult Create( ProjectEditorViewModel model )
      //{
      //   Validate( model, TransactionType.Insert );

      //   if (ModelState.IsValid)
      //   {
      //      Repository.Add( new Project( model ) );
      //      return RedirectToAction( () => this.Index() );
      //   }

      //   model.ProjectStatuses = _projectStatusRepository.GetAll().ToSelectList();
      //   return View( model );
      //}

      ////
      //// GET: /Projects/Edit/Guid
      //public override ActionResult Edit( Guid id )
      //{
      //   var model = Repository.Get( id );

      //   if (model != null)
      //   {
      //      var viewModel = new ProjectEditorViewModel( model );
      //      viewModel.ProjectStatuses = _projectStatusRepository.GetAll().ToSelectList( viewModel.ProjectStatus.Id );
      //      return View( viewModel );
      //   }

      //   return HttpNotFound();
      //}

      ////
      //// POST: /Projects/Edit/Guid
      //[HttpPost]
      //public virtual ActionResult Edit( ProjectEditorViewModel model )
      //{
      //   Validate( model, TransactionType.Update );

      //   if (ModelState.IsValid)
      //   {
      //      Repository.Update( new Project( model ) );
      //      return RedirectToAction( () => this.Index() );
      //   }

      //   return View( model );
      //}
   }
}