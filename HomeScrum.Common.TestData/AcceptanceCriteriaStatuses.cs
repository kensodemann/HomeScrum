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
            Id=Guid.NewGuid(),
            Name="Unverified",
            Description="Not yet verified",
            StatusCd='A',
            IsAccepted='N',
            IsPredefined='Y'
         },
         new AcceptanceCriteriaStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Accepted",
            Description="Accepted",
            StatusCd='A',
            IsAccepted='Y',
            IsPredefined='Y'
         },
         new AcceptanceCriteriaStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Rejected",
            Description="Rejected",
            StatusCd='A',
            IsAccepted='N',
            IsPredefined='Y'
         },
         new AcceptanceCriteriaStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Accepted, Predefined",
            StatusCd='A',
            IsAccepted='Y',
            IsPredefined='Y'
         },
         new AcceptanceCriteriaStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 2",
            Description="Inactive Status, Is Accepted, Predefined",
            StatusCd='I',
            IsAccepted='Y',
            IsPredefined='Y'
         },
         new AcceptanceCriteriaStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 3",
            Description="Active Status, Is Not Accepted, Predefined",
            StatusCd='A',
            IsAccepted='N',
            IsPredefined='Y'
         },
         new AcceptanceCriteriaStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 4",
            Description="Active Status, Is Accepted, Not Predefined",
            StatusCd='A',
            IsAccepted='Y',
            IsPredefined='N'
         }
      };
   }
}
