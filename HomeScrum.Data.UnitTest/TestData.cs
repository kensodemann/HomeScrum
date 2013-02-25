using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.UnitTest
{
   public class TestData
   {
      private static ISessionFactory _sessionFactory;
      private static Configuration _configuration;

      public static void Initialize()
      {
         _configuration = new Configuration();
         _configuration.Configure();
         _configuration.AddAssembly( typeof( WorkItemType ).Assembly );
         _sessionFactory = _configuration.BuildSessionFactory();
      }


      public static void BuildDatabase()
      {
         new SchemaExport( _configuration ).Execute( false, true, false );
         CreateInitialWorkItemStatuses();
         CreateInitialWorkItemTypes();
      }

      #region WorkItemStatuses
      private static void CreateInitialWorkItemStatuses()
      {

         using (ISession session = _sessionFactory.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItemStatus in WorkItemStatuses)
               session.Save( workItemStatus );
            transaction.Commit();
         }
      }

      public static readonly WorkItemStatus[] WorkItemStatuses = new[]
      {
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="New",
            Description="The Item is brand new",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="In Process",
            Description="The Item is being worked on",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="On Hold",
            Description="The Item was started but cannot be worked on",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Ready for Test",
            Description="The Item is ready to be tested",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Complete",
            Description="The Item is done",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Active, Predefined",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 2",
            Description="Inactive Status, Active, Predefined",
            StatusCd='I',
            IsActive='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Not Active, Predefined",
            StatusCd='A',
            IsActive='N',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Active, Not Predefined",
            StatusCd='A',
            IsActive='Y',
            IsPredefined='N'
         }
      };
      #endregion

      #region WorkItemTypes
      private static void CreateInitialWorkItemTypes()
      {

         using (ISession session = _sessionFactory.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItemType in WorkItemTypes)
               session.Save( workItemType );
            transaction.Commit();
         }
      }

      public static readonly WorkItemType[] WorkItemTypes = new[]
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
      #endregion
   }
}
