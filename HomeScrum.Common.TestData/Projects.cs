using HomeScrum.Common.Test.Utility;
using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Linq;

namespace HomeScrum.Common.TestData
{
   public class Projects
   {
      public static void Load( ISessionFactory sessionFactory )
      {
         LoadDependencies( sessionFactory );

         var session = sessionFactory.GetCurrentSession();

         if (!session.DataAlreadyLoaded<Project>())
         {
            CreateTestModelData( sessionFactory );
            session.LoadIntoDatabase( ModelData );
         }
      }

      private static void LoadDependencies( ISessionFactory sessionFactory ) { }

      public static Project[] ModelData { get; private set; }

      private static void CreateTestModelData( ISessionFactory sessionFactory, bool initializeIds = false )
      {
         var open = ProjectStatuses.ModelData.First( x => x.Name == "Open" );
         var inactive = ProjectStatuses.ModelData.First( x => x.Name == "Inactive" );
         var closed = ProjectStatuses.ModelData.First( x => x.Name == "Closed" );

         ModelData = new[]
         {
            new Project( sessionFactory )
            {
               Name="Home Scrum",
               Description = "This project right here",
               Status = open,
               LastModifiedUserRid = Users.ModelData[0].Id
            },
            new Project( sessionFactory )
            {
               Name="PRepS",
               Description = "An old problem reporting system",
               Status = closed,
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Project( sessionFactory )
            {
               Name="MathWar",
               Description = "A flash card math learning game",
               Status = inactive,
               LastModifiedUserRid = Users.ModelData[2].Id
            },
            new Project( sessionFactory )
            {
               Name="Sandwiches",
               Description = "Make them!",
               Status = open,
               LastModifiedUserRid = Users.ModelData[0].Id
            },
            new Project( sessionFactory )
            {
               Name="TacoBell",
               Description="Make some tacos",
               Status=ProjectStatuses.ModelData.First( x => x.StatusCd == 'I' ),
               LastModifiedUserRid = Users.ModelData[0].Id
            }
         };

         if (initializeIds)
         {
            InitializeIds();
         }
      }

      private static void InitializeIds()
      {
         foreach (var model in ModelData)
         {
            model.Id = Guid.NewGuid();
         }
      }
   }
}
