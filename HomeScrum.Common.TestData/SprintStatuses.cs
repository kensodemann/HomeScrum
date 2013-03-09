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

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static readonly SprintStatus[] ModelData = new[]
      {
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Future",
            Description="The sprint is set up for the future",
            StatusCd='A',
            IsOpenStatus='N',
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Planning",
            Description="In Planning",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Active",
            Description="The sprint is the active one",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Complete",
            Description="The sprint is done",
            StatusCd='A',
            IsOpenStatus='N',
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Predefined",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 2",
            Description="Inactive Status, Is Open, Predefined",
            StatusCd='I',
            IsOpenStatus='Y',
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Not Open, Predefined",
            StatusCd='A',
            IsOpenStatus='N',
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Not Predefined",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined=false
         }
      };
   }
}
