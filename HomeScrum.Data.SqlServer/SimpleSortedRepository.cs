using HomeScrum.Data.SqlServer.Helpers;
using HomeScrum.Services;
using NHibernate;
using NHibernate.Criterion;
using Ninject;
using System.Collections.Generic;

namespace HomeScrum.Data.SqlServer
{
   public class SimpleSortedRepository<T> : Repository<T>
   {
      [Inject]
      public SimpleSortedRepository( ILogger logger ) : base( logger ) { }

      public override ICollection<T> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( T ) )
               .AddOrder( Order.Asc( "SortSequence" ) )
               .List<T>();
         }
      }
   }
}
