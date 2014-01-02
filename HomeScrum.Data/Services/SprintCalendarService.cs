using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;

namespace HomeScrum.Data.Services
{
   public class SprintCalendarService : ISprintCalendarService
   {
      private readonly ILogger _logger;
      private readonly ISessionFactory _sessionFactory;
      private IEnumerable<WorkItem> _sprintTasks;

      public SprintCalendarService( ILogger logger, ISessionFactory sessionFactory )
      {
         _logger = logger;
         _sessionFactory = sessionFactory;
      }

      public void Update( Sprint sprint )
      {
         Log.Debug( "Updating Sprint Calendar" );

         if (sprint.StartDate == null || sprint.EndDate == null)
         {
            return;
         }

         GetSprintTasks( sprint );

         var date = ((DateTime)sprint.StartDate);
         while (date < DateTime.Now.Date)
         {
            if (sprint.Calendar.FirstOrDefault( x => x.HistoryDate == date ) == null)
            {
               CreateCalendarEntry( sprint, date );
            }
            date = date.AddDays( 1 );
         }
         CreateCalendarEntry( sprint, DateTime.Now.Date );

         Log.Debug( "Sprint Calendar Update Complete" );
      }

      private void GetSprintTasks( Sprint sprint )
      {
         var _session = _sessionFactory.GetCurrentSession();
         _sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();
      }

      private void CreateCalendarEntry( Sprint sprint, DateTime date )
      {
         var p = PointsRemaining( sprint, date );
         var s = new SprintCalendarEntry()
         {
            Sprint = sprint,
            HistoryDate = date,
            PointsRemaining = p
         };
         sprint.Calendar.Add( s );
      }

      private int PointsRemaining( Sprint sprint, DateTime date )
      {
         int points = 0;
         foreach (var task in _sprintTasks)
         {
            var snapshot = task.PointsHistory.LastOrDefault( x => x.HistoryDate <= date );
            if (snapshot != null)
            {
               points += snapshot.PointsRemaining;
            }
         }

         return points;
      }

      protected ILogger Log
      {
         get { return _logger; }
      }
   }
}
