using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;

namespace HomeScrum.Common.TestData
{
   public class AcceptanceCriteriaStatuses
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<AcceptanceCriterionStatus>())
         {
            CreateTestModelData( sessionFactory );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory ) { }

      public static AcceptanceCriterionStatus[] ModelData { get; private set; }

      private static void CreateTestModelData(ISessionFactory sessionFactory, bool initializeIds = false )
      {
         ModelData = new[]
         {
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="Unverified",
               Description="Not yet verified",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=true,
               SortSequence=1
            }, 
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="Accepted",
               Description="This criteria has been met.",
               StatusCd='A',
               IsAccepted=true,
               IsPredefined=true,
               SortSequence=2
            },
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="Rejected",
               Description="This criteria has not been met.",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=true,
               SortSequence=3
            },
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="In Test",
               Description="This criteria is currently being tested.",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=false,
               SortSequence=4
            },
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="Inconclusive",
               Description="It is not possible to test this criteria",
               StatusCd='I',
               IsAccepted=false,
               IsPredefined=false,
               SortSequence=5
            },
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="Open",
               Description="Criteria is open",
               StatusCd='I',
               IsAccepted=false,
               IsPredefined=false,
               SortSequence=6
            },
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="Closed",
               Description="Criteria is closed",
               StatusCd='I',
               IsAccepted=true,
               IsPredefined=false,
               SortSequence=7
            },
            new AcceptanceCriterionStatus( sessionFactory )
            {
               Name="Inactive",
               Description="Criteria is inactive",
               StatusCd='I',
               IsAccepted=false,
               IsPredefined=false,
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
