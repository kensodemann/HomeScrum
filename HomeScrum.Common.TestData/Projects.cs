using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Linq;

namespace HomeScrum.Common.TestData
{
   public class Projects
   {
      public static void Load()
      {
         CreateTestModelData();

         using (ISession session = Database.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var project in ModelData)
               session.Save( project );
            transaction.Commit();
         }
      }

      public static Project[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         var open = ProjectStatuses.ModelData.First( x => x.Name == "Open" );
         var inactive = ProjectStatuses.ModelData.First( x => x.Name == "Inactive" );
         var closed = ProjectStatuses.ModelData.First( x => x.Name == "Closed" );

         ModelData = new[]
         {
            new Project ()
            {
               Name="Home Scrum",
               Description = "This project right here",
               Status = open,
               LastModifiedUserRid = Users.ModelData[0].Id
            },
            new Project ()
            {
               Name="PRepS",
               Description = "An old problem reporting system",
               Status = closed,
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Project ()
            {
               Name="MathWar",
               Description = "A flash card math learning game",
               Status = inactive,
               LastModifiedUserRid = Users.ModelData[2].Id
            },
            new Project ()
            {
               Name="Sandwiches",
               Description = "Make them!",
               Status = open,
               LastModifiedUserRid = Users.ModelData[0].Id
            },
            new Project()
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
