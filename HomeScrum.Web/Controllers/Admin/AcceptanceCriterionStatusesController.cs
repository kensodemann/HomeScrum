using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.Controllers.Admin
{
   public class AcceptanceCriterionStatusesController : SystemDataObjectController<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel>
   {
      [Inject]
      public AcceptanceCriterionStatusesController( IValidator<AcceptanceCriterionStatus> validator,
         IPropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( validator, translator, logger, sessionFactory ) { }
   }
}
