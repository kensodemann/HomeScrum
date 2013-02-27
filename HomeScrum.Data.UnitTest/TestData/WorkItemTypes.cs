using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.UnitTest.TestData
{
   class WorkItemTypes
   {
      public static void Load()
      {

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItemType in ModelData)
               session.Save( workItemType );
            transaction.Commit();
         }
      }

      public static readonly WorkItemType[] ModelData = new[]
      {
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="SBI",
            Description="Sprint Backlog Item",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="PBI",
            Description="Product BacklogItem",
            StatusCd='A',
            IsTask='N',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Bug",
            Description="A problem with the software or design",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Issue",
            Description="A problem in the process that is blocking someone",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 3",
            Description="Active, Not a Task, Predefined",
            StatusCd='A',
            IsTask='N',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 4",
            Description="Active, Task, Not Predefined",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='N'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 5",
            Description="Not Active, Task, Predefined",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='Y'
         }
      };
   }
}
