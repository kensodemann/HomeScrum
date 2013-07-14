using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Criterion;
using Ninject;
using Ninject.Extensions.Logging;
using System.Collections.Generic;

namespace HomeScrum.Data.SqlServer
{
   public class ProjectRepository : Repository<Project>
   {
      [Inject]
      public ProjectRepository( ILogger logger ) : base( logger ) { }

      public override ICollection<Project> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( Project ) )
               .CreateAlias( "Status", "stat" )
               .AddOrder( Order.Asc( "stat.SortSequence" ) )
               .AddOrder( Order.Asc( "Name" ) )
               .List<Project>();
         }
      }
   }
}
