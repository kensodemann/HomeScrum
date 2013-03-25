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
         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static readonly ProjectStatus[] ModelData = new[]
      {
         new ProjectStatus ()
         {
            Name="Active",
            Description="Active Project",
            AllowUse=true,
            IsActive=true,
            IsPredefined=true
         },
         new ProjectStatus ()
         {
            Name="Inactive",
            Description="No longer active",
            AllowUse=true,
            IsActive=false,
            IsPredefined=true
         },
         new ProjectStatus ()
         {
            Name="Closed",
            Description="The project is closed",
            AllowUse=true,
            IsActive=false,
            IsPredefined=true
         },
         new ProjectStatus ()
         {
            Name="Status 1",
            Description="Active Status, Is Active, Predefined",
            AllowUse=true,
            IsActive=true,
            IsPredefined=true
         },
         new ProjectStatus ()
         {
            Name="Status 2",
            Description="Inactive Status, Is Active, Predefined",
            AllowUse=false,
            IsActive=true,
            IsPredefined=true
         },
         new ProjectStatus ()
         {
            Name="Status 3",
            Description="Active Status, Is Not Active, Predefined",
            AllowUse=true,
            IsActive=false,
            IsPredefined=true
         },
         new ProjectStatus ()
         {
            Name="Status 4",
            Description="Active Status, Is Active, Not Predefined",
            AllowUse=true,
            IsActive=true,
            IsPredefined=false
         }
      };
   }
}
