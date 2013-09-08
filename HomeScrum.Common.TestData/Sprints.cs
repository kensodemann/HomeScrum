using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Linq;

namespace HomeScrum.Common.TestData
{
   /// <summary>
   /// Sprints are:
   ///   * Home Scrum Sprint 1 - closed, start and end dates
   ///   * Home Scrum Sprint 2 - closed, start and end dates
   ///   * Home Scrum Sprint 3 - retrospective, start and end dates
   ///   * Home Scrum Sprint 4 - planning, start date, no end date
   ///   * Home Scrum Sprint 5 - pre-planninig, no start or end date
   ///   
   ///   * Sandwiches Sprint 1 - pre-planning, no start or end date
   ///   * Sandwicnes Sprint 2 - in process, start date, no end date, same period as Home Scrum Sprint #3 (Now)
   ///   
   ///   * PRepS Sprint 1 - closed, start and end dates
   ///   * PRepS Sprint 2 - closed, start and end dates
   ///   * PRepS Sprint 3 - in process, start date, no end date
   ///   
   ///   * Valid Project Inactive Sprint Status
   ///   * Invalid Project Valid Sprint Status
   /// </summary>
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
         var homeScrum = Projects.ModelData.First( x => x.Name == "Home Scrum" );
         var sandwiches = Projects.ModelData.First( x => x.Name == "Sandwiches" );
         var preps = Projects.ModelData.First( x => x.Name == "PRepS" );
         var tacoBell = Projects.ModelData.First( x => x.Name == "TacoBell" );

         var prePlanning = SprintStatuses.ModelData.First( x => x.Name == "Pre Planning" );
         var planning = SprintStatuses.ModelData.First( x => x.Name == "Planning" );
         var inProcess = SprintStatuses.ModelData.First( x => x.Name == "In Process" );
         var retrospective = SprintStatuses.ModelData.First( x => x.Name == "Retrospective" );
         var closed = SprintStatuses.ModelData.First( x => x.Name == "Closed" );
         var invalidStatus = SprintStatuses.ModelData.First( x => x.StatusCd != 'A' );

         ModelData = new[]
         {
            // NOTE: A couple of these items are intentionally moved around and mixed in
            //       with sprints for other projects in order to test the ordering.
            new Sprint()
            {
               Name = "Home Scrum Sprint 1",
               Description = "The first sprint for the 1st active project",
               Status = closed,
               Project = homeScrum,
               StartDate = DateTime.Now.AddMonths(-3).AddDays(-30),
               EndDate = DateTime.Now.AddMonths(-3),
               Goal = "Get the initial design finalized",
               LastModifiedUserRid = Users.ModelData[0].Id,
               CreatedByUser = Users.ModelData[1]
            },
            new Sprint()
            {
               Name = "Home Scrum Sprint 3",
               Description = "The third sprint for the 1st active project",
               Status = retrospective,
               Project = homeScrum,
               StartDate = DateTime.Now.AddDays(-15),
               EndDate = DateTime.Now.AddDays(15),
               Goal = "Develop pattern for the controller classes",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[0]
            },
            new Sprint()
            {
               Name = "PRepS Sprint 2",
               Description = "The second sprint for the inactive project",
               Status = closed,
               Project = preps,
               StartDate = DateTime.Now.AddYears(-1),
               EndDate = DateTime.Now.AddYears(-1).AddDays(30),
               Goal = "Accomplish something else",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[2]
            },
            new Sprint()
            {
               Name = "Home Scrum Sprint 2",
               Description = "The second sprint for the 1st active project",
               Status = closed,
               Project = homeScrum,
               StartDate = DateTime.Now.AddMonths(-2).AddDays(-30),
               EndDate = DateTime.Now.AddMonths(-2),
               Goal = "Create Domains Models and ORM Mappings",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[1]
            },
            new Sprint()
            {
               Name = "Home Scrum Sprint 4",
               Description = "The forth sprint for the 1st active project",
               Status = planning,
               Project = homeScrum,
               StartDate = DateTime.Now.AddMonths(1),
               EndDate = null,
               Goal = "Expand controllers",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[2]
            },
            new Sprint()
            {
               Name = "Home Scrum Sprint 5",
               Description = "The fifth sprint for the 1st active project",
               Status = prePlanning,
               Project = homeScrum,
               StartDate = null,
               EndDate = null,
               Goal = null,
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[0]
            },
            new Sprint()
            {
               Name = "Sandwiches Sprint 1",
               Description = "The first sprint for the 2nd active project, oddly planning",
               Status = prePlanning,
               Project = sandwiches,
               StartDate = null,
               EndDate = null,
               Goal = "Create editor views",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[1]
            },
            new Sprint()
            {
               Name = "Sandwiches Sprint 2",
               Description = "The second sprint for the 2nd active project, oddly active",
               Status = inProcess,
               Project = sandwiches,
               StartDate = DateTime.Now.AddDays(-15),
               EndDate = null,
               Goal = "Test Null End Date",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[1]
            },
            new Sprint()
            {
               Name = "PRepS Sprint 1",
               Description = "The first sprint for the inactive project",
               Status = closed,
               Project = preps,
               StartDate = DateTime.Now.AddMonths(-13),
               EndDate = DateTime.Now.AddMonths(-12),
               Goal = "Accomplish something",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[0]
            },
            new Sprint()
            {
               Name = "PRepS Sprint 3",
               Description = "The third sprint for the inactive project",
               Status = inProcess,
               Project = preps,
               StartDate = DateTime.Now.AddMonths(-12),
               EndDate = null,
               Goal = "The project is imploding, attempt to prevent that",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[1]
            },
            new Sprint()
            {
               Name = "Valid Project Inactive Sprint Status",
               Description = "The project is valid, but the status is not valid for use in the system",
               Status = invalidStatus,
               Project = homeScrum,
               StartDate = DateTime.Now.AddMonths(-12),
               EndDate = null,
               Goal = "I don't know",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[1]
            },
            new Sprint()
            {
               Name = "Invalid Project Vaild Sprint Status",
               Description = "The third sprint for the inactive project",
               Status = inProcess,
               Project = tacoBell,
               StartDate = DateTime.Now.AddMonths(-12),
               EndDate = null,
               Goal = "I still don't know",
               LastModifiedUserRid = Users.ModelData[1].Id,
               CreatedByUser = Users.ModelData[1]
            }
         };
      }
   }
}
