using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Queries;
using NHibernate;
using NHibernate.Criterion;

namespace HomeScrum.Data.SqlServer.Queries
{
   public class WorkItemQuery : IDomainObjectQuery
   {
      public ICriteria GetQuery( ISession session )
      {
         var query = CreateBaseQuery( session );
         return query;
      }

      private ICriteria CreateBaseQuery( ISession session )
      {
         return session
            .CreateCriteria( typeof( WorkItem ) )
            .CreateAlias( "Status", "stat" )
            .CreateAlias( "WorkItemType", "wit" )
            .AddOrder( Order.Asc( "wit.SortSequence" ) )
            .AddOrder( Order.Asc( "stat.SortSequence" ) )
            .AddOrder( Order.Asc( "Name" ) );
      }
   }
}
