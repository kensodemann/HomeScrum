using System;
using System.Linq;
using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;

namespace HomeScrum.Data.Queries
{
   public static class WorkItemQueries
   {
      public static IQueryable<WorkItem> AllWorkItems( this ISession session )
      {
         return session.Query<WorkItem>()
            .OrderBy( x => x.Status.SortSequence )
            .ThenBy( x => x.WorkItemType.SortSequence )
            .ThenBy( x => x.Name.ToUpper() );
      }

      public static IQueryable<WorkItem> Backlog( this ISession session )
      {
         return AllWorkItems( session )
            .Where( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem );
      }

      public static IQueryable<WorkItem> Problems( this ISession session )
      {
         return AllWorkItems( session )
            .Where( x => x.WorkItemType.Category == WorkItemTypeCategory.Issue );
      }

      public static IQueryable<WorkItem> Tasks( this ISession session )
      {
         return AllWorkItems( session )
            .Where( x => x.WorkItemType.Category == WorkItemTypeCategory.Task );
      }

      public static IQueryable<WorkItem> Unassigned( this IQueryable<WorkItem> q )
      {
         return q.Where( x => (x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem && x.AssignedToUser == null) ||
                              (x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.Sprint == null) );
      }

      public static IQueryable<WorkItem> Mine( this IQueryable<WorkItem> q, Guid myId )
      {
         return q.Where( x => x.AssignedToUser != null && x.AssignedToUser.Id == myId );
      }
   }
}
