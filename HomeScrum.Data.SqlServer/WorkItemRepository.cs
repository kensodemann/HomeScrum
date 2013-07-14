using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using NHibernate;
using NHibernate.Criterion;
using Ninject;
using Ninject.Extensions.Logging;
using System.Collections.Generic;

namespace HomeScrum.Data.SqlServer
{
   public class WorkItemRepository : Repository<WorkItem>, IWorkItemRepository
   {
      [Inject]
      public WorkItemRepository( ILogger logger ) : base( logger ) { }

      public override ICollection<WorkItem> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateBaseQuery()
               .List<WorkItem>();
         }
      }

      public ICollection<WorkItem> GetAllProductBacklog()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateBaseQuery()
               .LimitToProductBacklogItems()
               .List<WorkItem>();
         }
      }

      public ICollection<WorkItem> GetOpenProductBacklog()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateBaseQuery()
               .LimitToProductBacklogItems()
               .LimitToOpenItems()
               .List<WorkItem>();
         }
      }
   }


   #region nHibernate Extensions
   internal static class WorkItemRepositoryCriteriaExtensions
   {
      public static ICriteria LimitToProductBacklogItems( this ICriteria queryCriteria )
      {
         return queryCriteria
            .Add( Expression.Eq( "wit.IsTask", false ) )
            .Add( Expression.Eq( "wit.StatusCd", 'A' ) );
      }

      public static ICriteria LimitToOpenItems( this ICriteria queryCriteria )
      {
         return queryCriteria
            .Add( Expression.Eq( "stat.IsOpenStatus", true ) )
            .Add( Expression.Eq( "stat.StatusCd", 'A' ) );
      }

      public static ICriteria CreateBaseQuery( this ISession session )
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
   #endregion
}
