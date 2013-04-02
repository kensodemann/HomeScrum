using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.TestData
{
   public class Projects
   {
      public static void Load()
      {
         CreateTestModelData();

         using (ISession session = Database.GetSession())
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
         ModelData = new[]
         {
            new Project ()
            {
               Name="Home Scrum",
               Description = "This project right here",
               ProjectStatus = ProjectStatuses.ModelData[0],
               LastModifiedUserRid = Users.ModelData[0].Id
            },
            new Project ()
            {
               Name="PRepS",
               Description = "An old problem reporting system",
               ProjectStatus = ProjectStatuses.ModelData[2],
               LastModifiedUserRid = Users.ModelData[1].Id
            },
            new Project ()
            {
               Name="MathWar",
               Description = "A flash card math learning game",
               ProjectStatus = ProjectStatuses.ModelData[1],
               LastModifiedUserRid = Users.ModelData[2].Id
            },
            new Project ()
            {
               Name="Sandwiches",
               Description = "Make them!",
               ProjectStatus = ProjectStatuses.ModelData[0],
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
