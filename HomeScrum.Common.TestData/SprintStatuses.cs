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

         using (ISession session = Database.OpenSession())
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
            new SprintStatus( Database.SessionFactory )
            {
               Name="Pre Planning",
               Description="The sprint is set up for the future",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=false,
               SortSequence=1
            },
            new SprintStatus( Database.SessionFactory )
            {
               Name="Planning",
               Description="In Planning",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=2
            },
            new SprintStatus( Database.SessionFactory )
            {
               Name="In Process",
               Description="The sprint is the active one",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=true,
               SortSequence=3
            },
            new SprintStatus( Database.SessionFactory )
            {
               Name="Retrospective",
               Description="The sprint is complete and a retrospective is being done",
               StatusCd='A',
               IsOpenStatus=true,
               IsPredefined=false,
               SortSequence=4
            },
            new SprintStatus( Database.SessionFactory )
            {
               Name="Closed",
               Description="The sprint and retrospective are both complete",
               StatusCd='A',
               IsOpenStatus=false,
               IsPredefined=true,
               SortSequence=5
            },
            new SprintStatus( Database.SessionFactory )
            {
               Name="Released",
               Description="The output of the sprint has been released to customers",
               StatusCd='I',
               IsOpenStatus=false,
               IsPredefined=false,
               SortSequence=6
            },
            new SprintStatus( Database.SessionFactory )
            {
               Name="New",
               Description="The sprint is newly created",
               StatusCd='I',
               IsOpenStatus=false,
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
