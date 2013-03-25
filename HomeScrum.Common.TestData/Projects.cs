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
         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var project in ModelData)
               session.Save( project );
            transaction.Commit();
         }
      }

      public static readonly Project[] ModelData = new[]
      {
         new Project ()
         {
            Id=Guid.NewGuid(),
            Name="Home Scrum",
            Description = "This project right here",
            ProjectStatus = ProjectStatuses.ModelData[0],
            LastModifiedUserId ="admin"
         },
         new Project ()
         {
            Id=Guid.NewGuid(),
            Name="PRepS",
            Description = "An old problem reporting system",
            ProjectStatus = ProjectStatuses.ModelData[2],
            LastModifiedUserId ="kws"
         },
         new Project ()
         {
            Id=Guid.NewGuid(),
            Name="MathWar",
            Description = "A flash card math learning game",
            ProjectStatus = ProjectStatuses.ModelData[1],
            LastModifiedUserId ="ams"
         },
         new Project ()
         {
            Id=Guid.NewGuid(),
            Name="Sandwiches",
            Description = "Make them!",
            ProjectStatus = ProjectStatuses.ModelData[0],
            LastModifiedUserId ="lls"
         }
      };
   }
}
