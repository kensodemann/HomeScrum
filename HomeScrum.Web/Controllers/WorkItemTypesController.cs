using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemTypesController : ReadWriteController<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel>
   {
      [Inject]
      public WorkItemTypesController( IRepository<WorkItemType> repository, IValidator<WorkItemType> validator, IPropertyNameTranslator<WorkItemTypeEditorViewModel> translator )
         : base( repository, validator, translator ) { }
   }
}
