using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer.Helpers;
using NHibernate;

namespace HomeScrum.Data.SqlServer
{
   public class SecurityRepository : ISecurityRepository
   {
      public bool IsValidLogin( string userId, string password )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            var result = session.GetNamedQuery( "ValidateUser" )
               .SetString( "userId", userId )
               .SetString( "password", password )
               .List();

            return result.Count == 1;
         }
      }

      public bool ChangePassword( string userId, string oldPassword, string newPassword )
      {
         if (!IsValidLogin( userId, oldPassword ))
         {
            return false;
         }

         using (ISession session = NHibernateHelper.OpenSession())
         {
            using (ITransaction transaction = session.BeginTransaction())
            {
               session.CreateSQLQuery( "update users " +
                                          "set password = hashbytes('SHA1', cast(:newPass as varchar(4000))) " +
                                        "where userId = :userId" )
                  .SetString( "userId", userId )
                  .SetString( "newPass", newPassword )
                  .ExecuteUpdate();
               transaction.Commit();
            }
         }

         return true;
      }
   }
}
