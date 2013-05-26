using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class AcceptanceCriteriaStatusesController : SystemDataObjectController<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatusEditorViewModel>
   {
      [Inject]
      public AcceptanceCriteriaStatusesController( IRepository<AcceptanceCriteriaStatus> repository, IValidator<AcceptanceCriteriaStatus> validator,
         IPropertyNameTranslator<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusEditorViewModel> translator )
         : base( repository, validator, translator ) { }
   }
}
