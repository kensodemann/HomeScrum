using System;
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
      public ProjectsController( IRepository<Project, Guid> repository, IRepository<ProjectStatus, Guid> projectStatusRepository, IValidator<Project> validator )
         : base( repository, validator )
      {
         _projectStatusRepository = projectStatusRepository;
      }

      private IRepository<ProjectStatus, Guid> _projectStatusRepository;

      public override ActionResult Create()
      {
         var model = new ProjectEditorViewModel();

         model.ProjectStatuses = _projectStatusRepository.GetAll().ToSelectList( default( Guid ) );

         return View( model );
      }
   }
}