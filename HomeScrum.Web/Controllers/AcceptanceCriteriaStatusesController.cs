﻿using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.Controllers
{
   public class AcceptanceCriteriaStatusesController : SystemDataObjectController<AcceptanceCriterionStatus, AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatusEditorViewModel>
   {
      [Inject]
      public AcceptanceCriteriaStatusesController( IRepository<AcceptanceCriterionStatus> repository, IValidator<AcceptanceCriterionStatus> validator,
         IPropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriteriaStatusEditorViewModel> translator, ILogger logger )
         : base( repository, validator, translator, logger ) { }
   }
}
