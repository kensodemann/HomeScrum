using HomeScrum.Data.Domain;
using HomeScrum.Data.SqlServer.Helpers;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Repositories;

namespace HomeScrum.Data.SqlServer
{
   public class WorkItemRepository : Repository<WorkItem>, IWorkItemRepository
   {
      public override ICollection<WorkItem> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( WorkItem ) )
               .CreateAlias( "Status", "stat" )
               .CreateAlias( "WorkItemType", "wit" )
               .AddOrder( Order.Asc( "wit.SortSequence" ) )
               .AddOrder( Order.Asc( "stat.SortSequence" ) )
               .AddOrder( Order.Asc( "Name" ) )
               .List<WorkItem>();
         }
      }

      public ICollection<WorkItem> GetAllProductBacklog()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( WorkItem ) )
               .CreateAlias( "Status", "stat" )
               .CreateAlias( "WorkItemType", "wit" )
               .Add( Expression.Eq( "wit.IsTask", false ) )
               .Add( Expression.Eq( "wit.StatusCd", 'A' ) )
               .AddOrder( Order.Asc( "wit.SortSequence" ) )
               .AddOrder( Order.Asc( "stat.SortSequence" ) )
               .AddOrder( Order.Asc( "Name" ) )
               .List<WorkItem>();
            // TODO: Need to add the sorting.
         }
      }

      public ICollection<WorkItem> GetOpenProductBacklog()
      {
         throw new NotImplementedException();
      }
   }
}
