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

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new WorkItemStatus ()
            {
               Name="New",
               Description="The Item is brand new",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=1
            },
            new WorkItemStatus ()
            {
               Name="In Process",
               Description="The Item is being worked on",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=2
            },
            new WorkItemStatus ()
            {
               Name="On Hold",
               Description="The Item was started but cannot be worked on",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=3
            },
            new WorkItemStatus ()
            {
               Name="Ready for Test",
               Description="The Item is ready to be tested",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=4
            },
            new WorkItemStatus ()
            {
               Name="Complete",
               Description="The Item is done",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=5
            },
            new WorkItemStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Open, Predefined",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=6
            },
            new WorkItemStatus ()
            {
               Name="Status 2",
               Description="Inactive Status, Is Open, Predefined",
               StatusCd='I',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=7
            },
            new WorkItemStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Not Open, Predefined",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=true,
               SortSequence=8
            },
            new WorkItemStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Open, Not Predefined",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=9
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
