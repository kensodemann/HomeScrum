using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;
using NHibernate;

namespace HomeScrum.Common.TestData
{
   public static class Users
   {
      public static void Load()
      {
         CreateTestModelData();

         using (ISession session = Database.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static User[] ModelData { get; private set; }

      public static void CreateTestModelData( bool initializeIds = false )
      {
         ModelData = new[]
         {
            new User()
            {
               UserName = "kws",
               LastName = "Smith",
               FirstName = "Kevin",
               MiddleName="William",
               StatusCd = 'A'
            },
            new User()
            {
               UserName = "j_a",
               LastName = "Anderson",
               FirstName = "Judy",
               StatusCd = 'A'
            },
            new User()
            {
               UserName = "q__",
               FirstName = "Quintin",
               StatusCd = 'A'
            },
            new User()
            {
               UserName = "iai",
               FirstName = "I",
               MiddleName = "Am",
               LastName= "Inactive",
               StatusCd = 'I'
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
