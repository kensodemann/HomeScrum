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

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static readonly AcceptanceCriteriaStatus[] ModelData = new[]
      {
         new AcceptanceCriteriaStatus ()
         {
            Name="Unverified",
            Description="Not yet verified",
            AllowUse=true,
            IsAccepted=false,
            IsPredefined=true
         },
         new AcceptanceCriteriaStatus ()
         {
            Name="Accepted",
            Description="Accepted",
            AllowUse=true,
            IsAccepted=true,
            IsPredefined=true
         },
         new AcceptanceCriteriaStatus ()
         {
            Name="Rejected",
            Description="Rejected",
            AllowUse=true,
            IsAccepted=false,
            IsPredefined=true
         },
         new AcceptanceCriteriaStatus ()
         {
            Name="Status 1",
            Description="Active Status, Is Accepted, Predefined",
            AllowUse=true,
            IsAccepted=true,
            IsPredefined=true
         },
         new AcceptanceCriteriaStatus ()
         {
            Name="Status 2",
            Description="Inactive Status, Is Accepted, Predefined",
            AllowUse=false,
            IsAccepted=true,
            IsPredefined=true
         },
         new AcceptanceCriteriaStatus ()
         {
            Name="Status 3",
            Description="Active Status, Is Not Accepted, Predefined",
            AllowUse=true,
            IsAccepted=false,
            IsPredefined=true
         },
         new AcceptanceCriteriaStatus ()
         {
            Name="Status 4",
            Description="Active Status, Is Accepted, Not Predefined",
            AllowUse=true,
            IsAccepted=true,
            IsPredefined=false
         }
      };
   }
}
