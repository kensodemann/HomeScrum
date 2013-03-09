using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class WorkItemStatuses
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

      public static readonly WorkItemStatus[] ModelData = new[]
      {
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="New",
            Description="The Item is brand new",
            StatusCd='A',
            IsOpenStatus=true,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="In Process",
            Description="The Item is being worked on",
            StatusCd='A',
            IsOpenStatus=true,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="On Hold",
            Description="The Item was started but cannot be worked on",
            StatusCd='A',
            IsOpenStatus=true,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Ready for Test",
            Description="The Item is ready to be tested",
            StatusCd='A',
            IsOpenStatus=true,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Complete",
            Description="The Item is done",
            StatusCd='A',
            IsOpenStatus=true,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Predefined",
            StatusCd='A',
            IsOpenStatus=true,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 2",
            Description="Inactive Status, Is Open, Predefined",
            StatusCd='I',
            IsOpenStatus=true,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Not Open, Predefined",
            StatusCd='A',
            IsOpenStatus=false,
            IsPredefined=true
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Not Predefined",
            StatusCd='A',
            IsOpenStatus=true,
            IsPredefined=false
         }
      };
   }
}
