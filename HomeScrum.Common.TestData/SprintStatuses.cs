using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class SprintStatuses
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         CreateTestModelData( sessionFactory );

         var session = sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
         session.Clear();
      }

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
               IsOpenStatus = false,
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
               IsOpenStatus = true,
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
               IsOpenStatus = true,
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
               IsOpenStatus = true,
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
               IsOpenStatus = false,
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
               IsOpenStatus = false,
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
               IsOpenStatus = false,
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
