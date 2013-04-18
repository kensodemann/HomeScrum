using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class WorkItems
   {
      public static void Load()
      {
         CreateTestModelData();

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItem in ModelData)
               session.Save( workItem );
            transaction.Commit();
         }
      }

      public static WorkItem[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new WorkItem()
            {
               Name = "Work Item #1",
               Description = "Description #1",
               WorkItemType = WorkItemTypes.ModelData[0],
               Status = WorkItemStatuses.ModelData[0],
               CreatedByUserRid = Users.ModelData[0].Id,
               LastModifiedUserRid = Users.ModelData[0].Id,
               Project = Projects.ModelData[1]
            }
         };

         if (initializeIds)
         {
            InitializeIds();
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
