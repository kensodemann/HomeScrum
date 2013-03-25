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
            Name="Future",
            Description="The sprint is set up for the future",
            AllowUse=true,
            IsOpenStatus=false,
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Name="Planning",
            Description="In Planning",
            AllowUse=true,
            IsOpenStatus=true,
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Name="Active",
            Description="The sprint is the active one",
            AllowUse=true,
            IsOpenStatus=true,
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Name="Complete",
            Description="The sprint is done",
            AllowUse=true,
            IsOpenStatus=false,
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Name="Status 1",
            Description="Active Status, Is Open, Predefined",
            AllowUse=true,
            IsOpenStatus=true,
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Name="Status 2",
            Description="Inactive Status, Is Open, Predefined",
            AllowUse=false,
            IsOpenStatus=true,
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Name="Status 1",
            Description="Active Status, Is Not Open, Predefined",
            AllowUse=true,
            IsOpenStatus=false,
            IsPredefined=true
         },
         new SprintStatus ()
         {
            Name="Status 1",
            Description="Active Status, Is Open, Not Predefined",
            AllowUse=true,
            IsOpenStatus=true,
            IsPredefined=false
         }
      };
   }
}
