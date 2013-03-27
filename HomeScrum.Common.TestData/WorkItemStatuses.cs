using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class WorkItemStatuses
   {
      public static void Load()
      {
         CreateTestModelData();

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static WorkItemStatus[] ModelData { get; private set; }

      public static void CreateTestModelData()
      {
         ModelData = new[]
         {
            new WorkItemStatus ()
            {
               Name="New",
               Description="The Item is brand new",
               AllowUse=true,
               IsOpenStatus=true,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="In Process",
               Description="The Item is being worked on",
               AllowUse=true,
               IsOpenStatus=true,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="On Hold",
               Description="The Item was started but cannot be worked on",
               AllowUse=true,
               IsOpenStatus=true,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="Ready for Test",
               Description="The Item is ready to be tested",
               AllowUse=true,
               IsOpenStatus=true,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="Complete",
               Description="The Item is done",
               AllowUse=true,
               IsOpenStatus=true,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Open, Predefined",
               AllowUse=true,
               IsOpenStatus=true,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="Status 2",
               Description="Inactive Status, Is Open, Predefined",
               AllowUse=false,
               IsOpenStatus=true,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Not Open, Predefined",
               AllowUse=true,
               IsOpenStatus=false,
               IsPredefined=true
            },
            new WorkItemStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Open, Not Predefined",
               AllowUse=true,
               IsOpenStatus=true,
               IsPredefined=false
            }
         };
      }
   }
}
