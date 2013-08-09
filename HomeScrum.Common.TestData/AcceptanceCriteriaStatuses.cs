using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class AcceptanceCriteriaStatuses
   {
      public static void Load()
      {
         CreateTestModelData(Database.SessionFactory);
         using (var session = Database.OpenSession())
         using (var transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static void Load(ISessionFactory sessionFactory)
      {
         CreateTestModelData(sessionFactory);
         var session = sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static AcceptanceCriterionStatus[] ModelData { get; private set; }

      public static void CreateTestModelData(ISessionFactory sessionFactory, bool initializeIds = false )
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
