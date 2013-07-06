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
         CreateTestModelData();
         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static AcceptanceCriterionStatus[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new AcceptanceCriterionStatus ()
            {
               Name="Unverified",
               Description="Not yet verified",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=true,
               SortSequence=1
            }, 
            new AcceptanceCriterionStatus ()
            {
               Name="Accepted",
               Description="This criteria has been met.",
               StatusCd='A',
               IsAccepted=true,
               IsPredefined=true,
               SortSequence=2
            },
            new AcceptanceCriterionStatus ()
            {
               Name="Rejected",
               Description="This criteria has not been met.",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=true,
               SortSequence=3
            },
            new AcceptanceCriterionStatus ()
            {
               Name="In Test",
               Description="This criteria is currently being tested.",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=false,
               SortSequence=4
            },
            new AcceptanceCriterionStatus ()
            {
               Name="Inconclusive",
               Description="It is not possible to test this criteria",
               StatusCd='I',
               IsAccepted=false,
               IsPredefined=false,
               SortSequence=5
            },
            new AcceptanceCriterionStatus ()
            {
               Name="Open",
               Description="Criteria is open",
               StatusCd='I',
               IsAccepted=false,
               IsPredefined=false,
               SortSequence=6
            },
            new AcceptanceCriterionStatus ()
            {
               Name="Closed",
               Description="Criteria is closed",
               StatusCd='I',
               IsAccepted=true,
               IsPredefined=false,
               SortSequence=7
            },
            new AcceptanceCriterionStatus ()
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
