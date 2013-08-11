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
         ModelData = new[]
         {
            new Sprint()
            {
               Name = "Project 0, Sprint 1",
               Description = "The first sprint for the 0th project in the list",
               Status = SprintStatuses.ModelData.First(x=>x.Name == "Closed"),
               Project = Projects.ModelData[0],
               StartDate = new DateTime(2013, 1, 1),
               EndDate = new DateTime(2013,1,31),
               Goal = "Get the initial design finalized",
               LastModifiedUserRid = Users.ModelData[0].Id
            },
            new Sprint()
            {
               Name = "Project 0, Sprint 2",
               Description = "The second sprint for the 0th project in the list",
               Status = SprintStatuses.ModelData.First(x=>x.Name == "Closed"),
               Project = Projects.ModelData[0],
               StartDate = new DateTime(2013, 2, 1),
               EndDate = new DateTime(2013,2,28),
               Goal = "Create Domains Models and ORM Mappings",
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Sprint()
            {
               Name = "Project 0, Sprint 3",
               Description = "The third sprint for the 0th project in the list",
               Status = SprintStatuses.ModelData.First(x=>x.Name == "Retrospective"),
               Project = Projects.ModelData[0],
               StartDate = new DateTime(2013, 3, 1),
               EndDate = new DateTime(2013,3,31),
               Goal = "Develop pattern for the controller classes",
               LastModifiedUserRid = Users.ModelData[1].Id
            }
         };
      }
   }
}
