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
   public class WorkItemStatusesController : SystemDataObjectController<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>
   {
      [Inject]
      public WorkItemStatusesController( IPropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }
   }
}
