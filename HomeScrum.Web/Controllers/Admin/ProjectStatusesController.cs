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
   public class ProjectStatusesController : SystemDataObjectController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>
   {
      [Inject]
      public ProjectStatusesController( IPropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger , sessionFactory) { }
   }
}
