using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class AcceptanceCriteriaStatusesController : ReadWriteController<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatusEditorViewModel>
   {
      [Inject]
      public AcceptanceCriteriaStatusesController( IRepository<AcceptanceCriteriaStatus> repository, IValidator<AcceptanceCriteriaStatus> validator )
         : base( repository, validator ) { }
   }
}
