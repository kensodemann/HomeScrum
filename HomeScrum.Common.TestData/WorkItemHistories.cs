using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;
using NHibernate;
using HomeScrum.Common.Test.Utility;

namespace HomeScrum.Common.TestData
{
   public class WorkItemHistories
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<WorkItemHistory>())
         {
            CreateTestModelData();
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory )
      {
         WorkItems.Load( sessionFactory );
      }

      private static List<WorkItemHistory> _workItemHistories;
      public static WorkItemHistory[] ModelData { get { return _workItemHistories.ToArray(); } }

      private static void CreateTestModelData( bool initializeIds = false )
      {
         _workItemHistories = new List<WorkItemHistory>();

         foreach(var item in WorkItems.ModelData)
         {
            CreateRandomHistory( item );
         }

         if (initializeIds)
         {
            InitializeIds();
         }
      }

      private static void CreateRandomHistory( WorkItem w )
      {
         var rnd = new Random();
         var entries = rnd.Next( 1, 20 );
         var prevEntryTimestamp = DateTime.Now;
         
         for (var i = 0; i < entries; i++)
         {
            var ago = rnd.Next( 1, 960 );
            prevEntryTimestamp = prevEntryTimestamp.AddHours(-1 * ago);
            var user = Users.ModelData[rnd.Next( 0, Users.ModelData.Count() )];

            var entry = new WorkItemHistory()
            {
               HistoryTimestamp = prevEntryTimestamp,
               SequenceNumber = i + 1,
               LastModifiedUser = user,
               WorkItem = w
            };

            _workItemHistories.Add( entry );
         }
      }

      private static void InitializeIds()
      {
         foreach (var model in ModelData)
         {
            model.Id = Guid.NewGuid();
         }
      }
   }
}
