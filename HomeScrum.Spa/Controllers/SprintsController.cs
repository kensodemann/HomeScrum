using HomeScrum.Data.Domain;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Spa.Controllers
{
   public class SprintsController:DomainObjectApiController<Sprint>
   {
      [Inject]
      public SprintsController( ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory ) { }
   }
}