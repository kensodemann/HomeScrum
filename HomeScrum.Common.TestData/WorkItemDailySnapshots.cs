using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;

namespace HomeScrum.Common.TestData
{
   public class WorkItemDailySnapshots
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<WorkItemDailySnapshot>())
         {
            CreateTestModelData();
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory )
      {
         WorkItems.Load( sessionFactory );
      }

      private static List<WorkItemDailySnapshot> _snapshotEntry;
      public static WorkItemDailySnapshot[] ModelData { get { return _snapshotEntry.ToArray(); } }

      private static void CreateTestModelData( bool initializeIds = false )
      {
         BuildBaseWorkItems();
      }

      private static void BuildBaseWorkItems()
      {
         _snapshotEntry = new List<WorkItemDailySnapshot>();

         var sprintTasks = WorkItems.ModelData.Where( x => x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem &&
            x.Sprint != null && x.Sprint.StartDate != null );

         foreach (var task in sprintTasks)
         {
            var rnd = new Random();
            var entries = rnd.Next( 1, 15 );
            var date = (DateTime)task.Sprint.StartDate;
            CreateSnapshotEntry( task, date, task.Points, task.Points, entries );
            for (var i = 0; i < entries - 1; i++)
            {
               var pointsRemaining = rnd.Next( 1, task.Points + 5 );
               date = date.AddDays( (double)rnd.Next( 1, 3 ) );
               CreateSnapshotEntry( task, date, task.Points, pointsRemaining, entries - 1 - i );
            }
         }
      }

      private static void CreateSnapshotEntry( WorkItem workItem, DateTime date, int points, int pointsRemaining, int sequence )
      {
         var entry = new WorkItemDailySnapshot()
         {
            HistoryDate = date,
            Points = points,
            PointsRemaining = pointsRemaining,
            SortSequenceNumber = sequence,
            WorkItem = workItem
         };

         _snapshotEntry.Add( entry );
      }
   }
}
