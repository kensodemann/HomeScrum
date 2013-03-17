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

         using (ISession session = Database.GetSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var status in ModelData)
               session.Save( status );
            transaction.Commit();
         }
      }

      public static readonly User[] ModelData = new[]
      {
         new User()
         {
            UserId = "kws",
            LastName = "Smith",
            FirstName = "Kevin",
            MiddleName="William",
            StatusCd = 'A'
         },
         new User()
         {
            UserId = "j_a",
            LastName = "Anderson",
            FirstName = "Judy",
            StatusCd = 'A'
         },
         new User()
         {
            UserId = "q__",
            FirstName = "Quintin",
            StatusCd = 'A'
         },
         new User()
         {
            UserId = "iai",
            FirstName = "I",
            MiddleName = "Am",
            LastName= "Inactive",
            StatusCd = 'I'
         }
      };
   }
}
