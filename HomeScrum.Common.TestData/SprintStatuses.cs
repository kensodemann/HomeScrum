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

      public static SprintStatus[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new SprintStatus ()
            {
               Name="Future",
               Description="The sprint is set up for the future",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=true
            },
            new SprintStatus ()
            {
               Name="Planning",
               Description="In Planning",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true
            },
            new SprintStatus ()
            {
               Name="Active",
               Description="The sprint is the active one",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true
            },
            new SprintStatus ()
            {
               Name="Complete",
               Description="The sprint is done",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=true
            },
            new SprintStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Open, Predefined",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true
            },
            new SprintStatus ()
            {
               Name="Status 2",
               Description="Inactive Status, Is Open, Predefined",
               StatusCd='I',
               IsOpenStatus=true,
               IsPredefined=true
            },
            new SprintStatus ()
            {
               Name="Status 3",
               Description="Active Status, Is Not Open, Predefined",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=true
            },
            new SprintStatus ()
            {
               Name="Status 4",
               Description="Active Status, Is Open, Not Predefined",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=false
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
