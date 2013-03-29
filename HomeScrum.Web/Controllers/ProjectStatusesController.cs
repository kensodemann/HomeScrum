using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Ninject;
using System;

namespace HomeScrum.Web.Controllers
{
   public class ProjectStatusesController : DomainObjectController<ProjectStatus, ProjectStatusEditorViewModel>
   {
      [Inject]
      public ProjectStatusesController( IRepository<ProjectStatus, Guid> repository, IValidator<ProjectStatus> validator )
         : base( repository, validator ) { }
   }
}
