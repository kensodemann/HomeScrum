using HomeScrum.Data.Domain;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Spa.Controllers
{
   public class AcceptanceCriterionStatusesController : DomainObjectApiController<AcceptanceCriterionStatus>
   {
      [Inject]
      public AcceptanceCriterionStatusesController( ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory ) { }
   }
}