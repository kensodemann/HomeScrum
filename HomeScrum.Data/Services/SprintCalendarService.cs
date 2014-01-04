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

         if (sprint.StartDate == null || sprint.EndDate == null ||
             ((DateTime)sprint.StartDate).Date > DateTime.Now.Date ||
             ((DateTime)sprint.EndDate).Date < DateTime.Now.Date)
         {
            return;
         }

         UpdateCalendar( sprint, DateTime.Now.Date );

         Log.Debug( "Sprint Calendar Update Complete" );
      }


      public void Reset( Sprint sprint )
      {
         Log.Debug( "Reset Sprint Calendar" );

         sprint.Calendar.Clear();

         if (sprint.StartDate == null || sprint.EndDate == null)
         {
            return;
         }

         var startDate = ((DateTime)sprint.StartDate).Date;
         var endDate = ((DateTime)sprint.EndDate).Date;
         UpdateCalendar( sprint,
            (DateTime.Now.Date < startDate) ? startDate :
            (DateTime.Now.Date > endDate) ? endDate :
            DateTime.Now.Date );

         Log.Debug( "Sprint Calendar Reset Complete" );
      }


      private void UpdateCalendar( Sprint sprint, DateTime throughDate )
      {
         GetSprintTasks( sprint );

         var date = ((DateTime)sprint.StartDate);
         while (date < throughDate)
         {
            if (sprint.Calendar.SingleOrDefault( x => x.HistoryDate == date ) == null)
            {
               CreateCalendarEntry( sprint, date );
            }
            date = date.AddDays( 1 );
         }
         CreateOrUpdateCalendarEntry( sprint, throughDate );
      }


      private void GetSprintTasks( Sprint sprint )
      {
         var _session = _sessionFactory.GetCurrentSession();
         _sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();
      }


      private void CreateOrUpdateCalendarEntry( Sprint sprint, DateTime date )
      {
         var entry = sprint.Calendar.SingleOrDefault( x => x.HistoryDate.Date == date.Date );
         if (entry != null)
         {
            entry.PointsRemaining = PointsRemaining( sprint, date );
         }
         else
         {
            CreateCalendarEntry( sprint, date );
         }
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
            var snapshot = task.PointsHistory
               .OrderBy( x => x.HistoryDate )
               .LastOrDefault( x => x.HistoryDate <= date );
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
