using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;

namespace HomeScrum.Common.TestData
{
   public class WorkItemStatuses
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<WorkItemStatus>())
         {
            CreateTestModelData( sessionFactory );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory ) { }

      public static WorkItemStatus[] ModelData { get; private set; }

      private static void CreateTestModelData( ISessionFactory sessionFactory, bool initializeIds = false )
      {
         ModelData = new[]
         {
            new WorkItemStatus( sessionFactory )
            {
               Name="New",
               Description="The Item is brand new",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=1
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="Planning",
               Description="The Item is in planning and estimation",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=2
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="Assigned",
               Description="The Item is assigned for work",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=3
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="In Process",
               Description="The Item is being worked on",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=4
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="On Hold",
               Description="The Item was started but cannot be worked on",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=5
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="Ready for Test",
               Description="The Item is ready to be tested",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=6
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="Complete",
               Description="The Item is done",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=true,
               SortSequence=7
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="Cancelled",
               Description="The Item is no longer needed",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=true,
               SortSequence=8
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="Waiting for Test",
               Description="The item is queued up to be tested.",
               StatusCd='I',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=9
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="In Design",
               Description="The task is being designed",
               StatusCd='I',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=10
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="In Functional",
               Description="Functional Specifications are being drawn up",
               StatusCd='I',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=11
            },
            new WorkItemStatus( sessionFactory )
            {
               Name="Estimating",
               Description="This task is being estimated",
               StatusCd='I',
               IsOpenStatus=false,
               IsPredefined=false,
               SortSequence=12
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
