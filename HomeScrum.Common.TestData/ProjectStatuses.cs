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
               Name="Active",
               Description="Active Project",
               StatusCd='A',
               IsActive=true,
               IsPredefined=true
            },
            new ProjectStatus ()
            {
               Name="Inactive",
               Description="No longer active",
               StatusCd='A',
               IsActive=false,
               IsPredefined=true
            },
            new ProjectStatus ()
            {
               Name="Closed",
               Description="The project is closed",
               StatusCd='A',
               IsActive=false,
               IsPredefined=true
            },
            new ProjectStatus ()
            {
               Name="Status 1",
               Description="Active Status, Is Active, Predefined",
               StatusCd='A',
               IsActive=true,
               IsPredefined=true
            },
            new ProjectStatus ()
            {
               Name="Status 2",
               Description="Inactive Status, Is Active, Predefined",
               StatusCd='I',
               IsActive=true,
               IsPredefined=true
            },
            new ProjectStatus ()
            {
               Name="Status 3",
               Description="Active Status, Is Not Active, Predefined",
               StatusCd='A',
               IsActive=false,
               IsPredefined=true
            },
            new ProjectStatus ()
            {
               Name="Status 4",
               Description="Active Status, Is Active, Not Predefined",
               StatusCd='A',
               IsActive=true,
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
