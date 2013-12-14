using HomeScrum.Data.Domain;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Spa.Controllers
{
   public class ProjectStatusesController : DomainObjectApiController<ProjectStatus>
   {
      [Inject]
      public ProjectStatusesController( ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory ) { }
   }
}