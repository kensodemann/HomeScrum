using HomeScrum.Data.Domain;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Spa.Controllers
{
   public class WorkItemStatusesController : DomainObjectApiController<WorkItemStatus>
   {
      [Inject]
      public WorkItemStatusesController( ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory ) { }
   }
}