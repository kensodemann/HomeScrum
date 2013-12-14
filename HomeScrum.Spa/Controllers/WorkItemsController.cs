using HomeScrum.Data.Domain;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Spa.Controllers
{
   public class WorkItemsController : DomainObjectApiController<WorkItem>
   {
      [Inject]
      public WorkItemsController( ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory ) { }
   }
}