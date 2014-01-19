using System;
using System.Linq;
using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;

namespace HomeScrum.Data.Queries
{
   public static class SprintQueries
   {
      public static IQueryable<Sprint> AllSprints( this ISession session )
      {
         return session.Query<Sprint>()
            .OrderBy( x => x.Project.Name )
            .ThenBy( x => x.StartDate ?? DateTime.MaxValue )
            .ThenBy( x => x.Status.SortSequence );
      }

      public static IQueryable<Sprint> OpenSprints( this ISession session )
      {
         return session.AllSprints()
            .Where( x => x.Status.StatusCd == 'A' && x.Status.Category != SprintStatusCategory.Complete );
      }

      public static IQueryable<Sprint> CurrentSprints( this ISession session )
      {
         return session.AllSprints()
            .Where( x => x.Status.StatusCd == 'A' && x.Status.Category == SprintStatusCategory.Active &&
               x.StartDate != null && x.StartDate <= DateTime.Now.Date && (x.EndDate == null || x.EndDate >= DateTime.Now.Date) );
      }
   }
}
