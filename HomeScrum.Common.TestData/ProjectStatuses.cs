using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class ProjectStatuses
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

      public static ProjectStatus[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new ProjectStatus ()
            {
               Name="Open",
               Description="Active Project",
               StatusCd='A',
               IsActive=true,
               IsPredefined=true,
               SortSequence=1
            },
            new ProjectStatus ()
            {
               Name="Inactive",
               Description="No longer active",
               StatusCd='A',
               IsActive=false,
               IsPredefined=false,
               SortSequence=2
            },
            new ProjectStatus ()
            {
               Name="Closed",
               Description="The project is closed",
               StatusCd='A',
               IsActive=false,
               IsPredefined=true,
               SortSequence=3
            },
            new ProjectStatus ()
            {
               Name="Waiting",
               Description="Awaiting Customer Approval",
               StatusCd='I',
               IsActive=false,
               IsPredefined=false,
               SortSequence=4
            },
            new ProjectStatus ()
            {
               Name="Deleted",
               Description="The project no longer exists",
               StatusCd='I',
               IsActive=true,
               IsPredefined=false,
               SortSequence=5
            },
            new ProjectStatus ()
            {
               Name="Active",
               Description="The project is active",
               StatusCd='I',
               IsActive=true,
               IsPredefined=false,
               SortSequence=6
            },
            new ProjectStatus ()
            {
               Name="On Hold",
               Description="The project is waiting for something",
               StatusCd='I',
               IsActive=true,
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
