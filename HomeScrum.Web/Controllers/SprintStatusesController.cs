using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class SprintStatusesController : ReadWriteController<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel>
   {
      [Inject]
      public SprintStatusesController( IRepository<SprintStatus> repository, IValidator<SprintStatus> validator )
         : base( repository, validator ) { }
   }
}
