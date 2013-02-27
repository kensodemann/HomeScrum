using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.UnitTest.TestData
{
   class ProjectStatuses
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
            Id=Guid.NewGuid(),
            Name="Active",
            Description="Active Project",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new ProjectStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Inactive",
            Description="No longer active",
            StatusCd='A',
            IsActive='N',
            IsPredefined='Y'
         },
         new ProjectStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Closed",
            Description="The project is closed",
            StatusCd='A',
            IsActive='N',
            IsPredefined='Y'
         },
         new ProjectStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Active, Predefined",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new ProjectStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 2",
            Description="Inactive Status, Is Active, Predefined",
            StatusCd='I',
            IsActive='Y',
            IsPredefined='Y'
         },
         new ProjectStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Not Active, Predefined",
            StatusCd='A',
            IsActive='N',
            IsPredefined='Y'
         },
         new ProjectStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Active, Not Predefined",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='N'
         }
      };
   }
}
