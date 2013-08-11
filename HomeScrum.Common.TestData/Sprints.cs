using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Linq;

namespace HomeScrum.Common.TestData
{
   public static class Sprints
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<Sprint>())
         {
            CreateTestModelData( sessionFactory );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory )
      {
         Users.Load( sessionFactory );
         Projects.Load( sessionFactory );
         SprintStatuses.Load( sessionFactory );
      }

      public static Sprint[] ModelData { get; private set; }

      private static void CreateTestModelData( ISessionFactory sessionFactory )
      {
         var project1 = Projects.ModelData.First( x => x.Status.IsActive );
         var project2 = Projects.ModelData.First( x => x.Status.IsActive && x.Id != project1.Id );
         var inactiveProject = Projects.ModelData.First( x => !x.Status.IsActive );
         var invalidProject = Projects.ModelData.First( x => x.Status.StatusCd != 'A' );

         var prePlanning = SprintStatuses.ModelData.First( x => x.Name == "Pre Planning" );
         var planning = SprintStatuses.ModelData.First( x => x.Name == "Planning" );
         var inProcess = SprintStatuses.ModelData.First( x => x.Name == "In Process" );
         var retrospective = SprintStatuses.ModelData.First( x => x.Name == "Retrospective" );
         var closed = SprintStatuses.ModelData.First( x => x.Name == "Closed" );
         var invalidStatus = SprintStatuses.ModelData.First( x => x.StatusCd != 'A' );

         ModelData = new[]
         {
            new Sprint()
            {
               Name = "Project 1, Sprint 1",
               Description = "The first sprint for the 1st active project",
               Status = closed,
               Project = project1,
               StartDate = new DateTime(2013, 1, 1),
               EndDate = new DateTime(2013,1,31),
               Goal = "Get the initial design finalized",
               LastModifiedUserRid = Users.ModelData[0].Id
            },
            new Sprint()
            {
               Name = "Project 1, Sprint 2",
               Description = "The second sprint for the 1st active project",
               Status = closed,
               Project = project1,
               StartDate = new DateTime(2013, 2, 1),
               EndDate = new DateTime(2013,2,28),
               Goal = "Create Domains Models and ORM Mappings",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Project 1, Sprint 3",
               Description = "The third sprint for the 1st active project",
               Status = retrospective,
               Project = project1,
               StartDate = new DateTime(2013, 3, 1),
               EndDate = new DateTime(2013,3,31),
               Goal = "Develop pattern for the controller classes",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Project 1, Sprint 4",
               Description = "The forth sprint for the 1st active project",
               Status = planning,
               Project = project1,
               StartDate = new DateTime(2013, 4, 1),
               EndDate = null,
               Goal = "Expand controllers",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Project 1, Sprint 5",
               Description = "The fifth sprint for the 1st active project",
               Status = prePlanning,
               Project = project1,
               StartDate = null,
               EndDate = null,
               Goal = null,
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Project 2, Sprint 1",
               Description = "The first sprint for the 2nd active project",
               Status = prePlanning,
               Project = project2,
               StartDate = null,
               EndDate = null,
               Goal = "Create editor views",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Inactive Project, Sprint 1",
               Description = "The first sprint for the inactive project",
               Status = closed,
               Project = inactiveProject,
               StartDate = new DateTime(2012, 6, 15),
               EndDate = new DateTime(2012,7,14),
               Goal = "Accomplish something",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Inactive Project, Sprint 2",
               Description = "The second sprint for the inactive project",
               Status = closed,
               Project = inactiveProject,
               StartDate = new DateTime(2012, 7, 15),
               EndDate = new DateTime(2012,8,14),
               Goal = "Accomplish something else",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Inactive Project, Sprint 3",
               Description = "The third sprint for the inactive project",
               Status = inProcess,
               Project = inactiveProject,
               StartDate = new DateTime(2012, 7, 15),
               EndDate = null,
               Goal = "The project is imploding, attempt to prevent that",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Valid Project, Inactive Sprint Status",
               Description = "The project is valid, but the status is not valid for use in the system",
               Status = invalidStatus,
               Project = project1,
               StartDate = new DateTime(2012, 7, 15),
               EndDate = null,
               Goal = "I don't know",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Invalid Project, Vaild Sprint Status",
               Description = "The third sprint for the inactive project",
               Status = inProcess,
               Project = invalidProject,
               StartDate = new DateTime(2012, 7, 15),
               EndDate = null,
               Goal = "I still don't know",
               LastModifiedUserRid = Users.ModelData[1].Id
            }
         };
      }
   }
}
