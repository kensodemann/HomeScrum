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

      public static AcceptanceCriteriaStatus[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new AcceptanceCriteriaStatus ()
            {
               Name="Unverified",
               Description="Not yet verified",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=true,
               SortSequence=1
            }, 
            new AcceptanceCriteriaStatus ()
            {
               Name="Accepted",
               Description="Accepted",
               StatusCd='A',
               IsAccepted=true,
               IsPredefined=true,
               SortSequence=2
            },
            new AcceptanceCriteriaStatus ()
            {
               Name="Rejected",
               Description="Rejected",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=true,
               SortSequence=3
            },
            new AcceptanceCriteriaStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Accepted, Predefined",
               StatusCd='A',
               IsAccepted=true,
               IsPredefined=true,
               SortSequence=4
            },
            new AcceptanceCriteriaStatus ()
            {
               Name="Status 2",
               Description="Inactive Status, Is Accepted, Predefined",
               StatusCd='I',
               IsAccepted=true,
               IsPredefined=true,
               SortSequence=5
            },
            new AcceptanceCriteriaStatus ()
            {
               Name="Status 3",
               Description="Active Status, Is Not Accepted, Predefined",
               StatusCd='A',
               IsAccepted=false,
               IsPredefined=true,
               SortSequence=6
            },
            new AcceptanceCriteriaStatus ()
            {
               Name="Status 4",
               Description="Active Status, Is Accepted, Not Predefined",
               StatusCd='A',
               IsAccepted=true,
               IsPredefined=false,
               SortSequence=7
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
