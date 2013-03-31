using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;

namespace HomeScrum.Web.Controllers
{
   public class ProjectController : DomainObjectViewModelController<Project, ProjectEditorViewModel>
   {
      public ProjectController( IRepository<Project, Guid> repository, IValidator<Project> validator )
         : base( repository, validator ) { }

      protected override ProjectEditorViewModel CreateViewModel( Project model )
      {
         return new ProjectEditorViewModel( model );
      }

      protected override Project CreateNewModel( Project viewModel )
      {
         return new Project( viewModel );
      }
   }
}