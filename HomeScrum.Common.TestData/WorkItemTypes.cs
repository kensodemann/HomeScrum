using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class WorkItemTypes
   {
      public static void Load()
      {
         CreateTestModelData();

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItemType in ModelData)
               session.Save( workItemType );
            transaction.Commit();
         }
      }

      public static WorkItemType[] ModelData { get; private set; }

      public static void CreateTestModelData()
      {
         ModelData = new[]
         {
            new WorkItemType ()
            {
               Name="SBI",
               Description="Sprint Backlog Item",
               AllowUse=true,
               IsTask=true,
               IsPredefined=true
            },
            new WorkItemType ()
            {
               Name="PBI",
               Description="Product BacklogItem",
               AllowUse=true,
               IsTask=false,
               IsPredefined=true
            },
            new WorkItemType ()
            {
               Name="Bug",
               Description="A problem with the software or design",
               AllowUse=true,
               IsTask=true,
               IsPredefined=true
            },
            new WorkItemType ()
            {
               Name="Issue",
               Description="A problem in the process that is blocking someone",
               AllowUse=true,
               IsTask=true,
               IsPredefined=true
            },
            new WorkItemType ()
            {
               Name="type 3",
               Description="Active, Not a Task, Predefined",
               AllowUse=true,
               IsTask=false,
               IsPredefined=true
            },
            new WorkItemType ()
            {
               Name="Type 4",
               Description="Active, Task, Not Predefined",
               AllowUse=true,
               IsTask=true,
               IsPredefined=false
            },
            new WorkItemType ()
            {
               Name="tYPe 5",
               Description="Not Active, Task, Predefined",
               AllowUse=false,
               IsTask=true,
               IsPredefined=true
            },
            new WorkItemType ()
            {
               Name="TYPE 6",
               Description="Not Active, Task, Predefined - second one line this",
               AllowUse=false,
               IsTask=true,
               IsPredefined=true
            }
         };
      }
   }
}
