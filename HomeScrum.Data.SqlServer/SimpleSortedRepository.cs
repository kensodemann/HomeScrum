using HomeScrum.Data.SqlServer.Helpers;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.SqlServer
{
   public class SimpleSortedRepository<T> : Repository<T>
   {
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
