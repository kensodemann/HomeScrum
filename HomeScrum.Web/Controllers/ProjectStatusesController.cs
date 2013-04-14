using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class ProjectStatusesController : ReadWriteController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>
   {
      [Inject]
      public ProjectStatusesController( IRepository<ProjectStatus> repository, IValidator<ProjectStatus> validator )
         : base( repository, validator ) { }
   }
}
