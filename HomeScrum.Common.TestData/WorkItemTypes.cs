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
               Name="PBI",
               Description="Product Backlog Item",
               StatusCd='A',
               IsTask=false,
               IsPredefined=true,
               SortSequence=5
            },
            new WorkItemType ()
            {
               Name="CR",
               Description="Customer Request",
               StatusCd='A',
               IsTask=false,
               IsPredefined=false,
               SortSequence=6
            },
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
               Name="Bug",
               Description="A problem with the software or design",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=2
            },
            new WorkItemType ()
            {
               Name="Issue",
               Description="A problem in the process that is blocking someone",
               StatusCd='A',
               IsTask=true,
               IsPredefined=true,
               SortSequence=3
            },
            new WorkItemType ()
            {
               Name="Design Goal",
               Description="The output of this task is a design item",
               StatusCd='A',
               IsTask=true,
               IsPredefined=false,
               SortSequence=4
            },
            new WorkItemType ()
            {
               Name="CFP",
               Description="Customer Funded PBI",
               StatusCd='I',
               IsTask=false,
               IsPredefined=false,
               SortSequence=7
            },
            new WorkItemType ()
            {
               Name="IFP",
               Description="Internally Funded PBI",
               StatusCd='I',
               IsTask=false,
               IsPredefined=false,
               SortSequence=8
            },
            new WorkItemType ()
            {
               Name="WO",
               Description="Work Order",
               StatusCd='I',
               IsTask=true,
               IsPredefined=false,
               SortSequence=9
            },
            new WorkItemType ()
            {
               Name="PL",
               Description="Problem Log",
               StatusCd='I',
               IsTask=true,
               IsPredefined=false,
               SortSequence=10
            },
            new WorkItemType ()
            {
               Name="Step",
               Description="A specific step required to complete and CFP or IFP",
               StatusCd='I',
               IsTask=true,
               IsPredefined=false,
               SortSequence=11
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
