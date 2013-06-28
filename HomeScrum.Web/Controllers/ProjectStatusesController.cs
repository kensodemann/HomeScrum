using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Services;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Ninject;

namespace HomeScrum.Web.Controllers
{
   public class ProjectStatusesController : SystemDataObjectController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>
   {
      [Inject]
      public ProjectStatusesController( IRepository<ProjectStatus> repository, IValidator<ProjectStatus> validator, IPropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel> translator, ILogger logger )
         : base( repository, validator, translator, logger ) { }
   }
}
