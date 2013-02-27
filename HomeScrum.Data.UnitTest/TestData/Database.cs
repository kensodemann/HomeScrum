using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;

namespace HomeScrum.Data.UnitTest.TestData
{
   public class Database
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


      public static void Build()
      {
         new SchemaExport( _configuration ).Execute( false, true, false );
         CreateInitialProjectStatuses();
         CreateInitialSprintStatuses();
         CreateInitialWorkItemStatuses();
         CreateInitialWorkItemTypes();
      }

      public static ISession GetSession()
      {
         return _sessionFactory.OpenSession();
      }

      #region ProjectStatuses
      private static void CreateInitialProjectStatuses()
      {

         using (ISession session = _sessionFactory.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ProjectStatuses)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static readonly ProjectStatus[] ProjectStatuses = new[]
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
      #endregion

      #region SprintStatuses
      private static void CreateInitialSprintStatuses()
      {

         using (ISession session = _sessionFactory.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in SprintStatuses)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static readonly SprintStatus[] SprintStatuses = new[]
      {
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Future",
            Description="The sprint is set up for the future",
            StatusCd='A',
            IsOpenStatus='N',
            IsPredefined='Y'
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Planning",
            Description="In Planning",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Active",
            Description="The sprint is the active one",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Complete",
            Description="The sprint is done",
            StatusCd='A',
            IsOpenStatus='N',
            IsPredefined='Y'
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Predefined",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 2",
            Description="Inactive Status, Is Open, Predefined",
            StatusCd='I',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Not Open, Predefined",
            StatusCd='A',
            IsOpenStatus='N',
            IsPredefined='Y'
         },
         new SprintStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Not Predefined",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='N'
         }
      };
      #endregion

      #region WorkItemStatuses
      private static void CreateInitialWorkItemStatuses()
      {

         using (ISession session = _sessionFactory.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in WorkItemStatuses)
               session.Save( status );
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
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="In Process",
            Description="The Item is being worked on",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="On Hold",
            Description="The Item was started but cannot be worked on",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Ready for Test",
            Description="The Item is ready to be tested",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Complete",
            Description="The Item is done",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Predefined",
            StatusCd='A',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 2",
            Description="Inactive Status, Is Open, Predefined",
            StatusCd='I',
            IsOpenStatus='Y',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Not Open, Predefined",
            StatusCd='A',
            IsOpenStatus='N',
            IsPredefined='Y'
         },
         new WorkItemStatus ()
         {
            Id=Guid.NewGuid(),
            Name="Status 1",
            Description="Active Status, Is Open, Not Predefined",
            StatusCd='A',
            IsOpenStatus='Y',
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
