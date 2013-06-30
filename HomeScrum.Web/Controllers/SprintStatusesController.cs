using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.Controllers
{
   public class SprintStatusesController : SystemDataObjectController<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel>
   {
      [Inject]
      public SprintStatusesController( IRepository<SprintStatus> repository, IValidator<SprintStatus> validator, IPropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel> translator, ILogger logger )
         : base( repository, validator, translator, logger ) { }
   }
}
