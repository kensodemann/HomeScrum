﻿using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemStatusesController : ReadWriteController<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>
   {
      [Inject]
      public WorkItemStatusesController( IRepository<WorkItemStatus> repository, IValidator<WorkItemStatus> validator, IPropertyNameTranslator<WorkItemStatusEditorViewModel> translator )
         : base( repository, validator, translator ) { }
   }
}
