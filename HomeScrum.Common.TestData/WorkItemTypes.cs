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

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new WorkItemType ()
            {
               Name="SBI",
               Description="Sprint Backlog Item",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=1
            },
            new WorkItemType ()
            {
               Name="PBI",
               Description="Product BacklogItem",
               StatusCd='A',
               IsTask=false,
               IsPredefined=true,
               SortSequence=2
            },
            new WorkItemType ()
            {
               Name="Bug",
               Description="A problem with the software or design",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=3
            },
            new WorkItemType ()
            {
               Name="Issue",
               Description="A problem in the process that is blocking someone",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=4
            },
            new WorkItemType ()
            {
               Name="type 3",
               Description="Active, Not a Task, Predefined",
               StatusCd='A',
               IsTask=false,
               IsPredefined=true,
               SortSequence=5
            },
            new WorkItemType ()
            {
               Name="Type 4",
               Description="Active, Task, Not Predefined",
               StatusCd='A',
               IsTask=true,
               IsPredefined=false,
               SortSequence=6
            },
            new WorkItemType ()
            {
               Name="tYPe 5",
               Description="Not Active, Task, Predefined",
               StatusCd='I',
               IsTask=true,
               IsPredefined=true,
               SortSequence=7
            },
            new WorkItemType ()
            {
               Name="TYPE 6",
               Description="Not Active, Task, Predefined - second one line this",
               StatusCd='I',
               IsTask=true,
               IsPredefined=true,
               SortSequence=8
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
