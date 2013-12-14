using HomeScrum.Data.Domain;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Spa.Controllers
{
   public class AcceptanceCriteriaController:DomainObjectApiController<AcceptanceCriterion>
   {
      [Inject]
      public AcceptanceCriteriaController( ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory ) { }
   }
}