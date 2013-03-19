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

      public void ChangePassword( string userId, string oldPassword, string newPassword )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            using (ITransaction transaction = session.BeginTransaction())
            {
               session.CreateSQLQuery( "update users " +
                                          "set password = hashbytes('SHA1', cast(:newPass as varchar(4000))) " +
                                        "where userId = :userId " +
                                          "and password = hashbytes('SHA1', cast(:oldPass as varchar(4000)))" )
                  .SetString( "userId", userId )
                  .SetString( "newPass", newPassword )
                  .SetString( "oldPass", oldPassword )
                  .ExecuteUpdate();
               transaction.Commit();
            }
         }
      }
   }
}
