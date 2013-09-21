using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;

namespace HomeScrum.Common.TestData
{
   public class SprintStatuses
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<SprintStatus>())
         {
            CreateTestModelData( sessionFactory );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory ) { }

      public static SprintStatus[] ModelData { get; private set; }

      private static void CreateTestModelData( ISessionFactory sessionFactory, bool initializeIds = false )
      {
         ModelData = new[]
         {
            new SprintStatus( sessionFactory )
            {
               Name = "Pre Planning",
               Description = "The sprint is set up for the future",
               StatusCd = 'A',
               Category = SprintStatusCategory.Inactive,
               IsPredefined = false,
               BacklogIsClosed = false,
               TaskListIsClosed = false,
               SortSequence = 1
            },
            new SprintStatus( sessionFactory )
            {
               Name = "Planning",
               Description = "In Planning",
               StatusCd = 'A',
               Category = SprintStatusCategory.Inactive,
               IsPredefined = true,
               BacklogIsClosed = false,
               TaskListIsClosed = false,
               SortSequence = 2
            },
            new SprintStatus( sessionFactory )
            {
               Name = "In Process",
               Description = "The sprint is the active one",
               StatusCd = 'A',
               Category = SprintStatusCategory.Active,
               IsPredefined = true,
               BacklogIsClosed = true,
               TaskListIsClosed = false,
               SortSequence = 3
            },
            new SprintStatus( sessionFactory )
            {
               Name = "Retrospective",
               Description = "The sprint is complete and a retrospective is being done",
               StatusCd = 'A',
               Category = SprintStatusCategory.Active,
               IsPredefined = false,
               BacklogIsClosed = true,
               TaskListIsClosed = true,
               SortSequence = 4
            },
            new SprintStatus( sessionFactory )
            {
               Name = "Closed",
               Description = "The sprint and retrospective are both complete",
               StatusCd = 'A',
               Category = SprintStatusCategory.Complete,
               IsPredefined = true,
               BacklogIsClosed = true,
               TaskListIsClosed = true,
               SortSequence = 5
            },
            new SprintStatus( sessionFactory )
            {
               Name = "Released",
               Description = "The output of the sprint has been released to customers",
               StatusCd = 'I',
               Category = SprintStatusCategory.Complete,
               IsPredefined = false,
               BacklogIsClosed = true,
               TaskListIsClosed = true,
               SortSequence = 6
            },
            new SprintStatus( sessionFactory )
            {
               Name = "New",
               Description = "The sprint is newly created",
               StatusCd = 'I',
               Category = SprintStatusCategory.Inactive,
               IsPredefined = false,
               BacklogIsClosed = false,
               TaskListIsClosed = false,
               SortSequence = 7
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
