using HomeScrum.Common.Utility;
using HomeScrum.Data.Repositories;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Data.Services
{
   public class SecurityService : ISecurityService
   {
      [Inject]
      public SecurityService( ILogger logger )
      {
         _logger = logger;
      }

      private readonly ILogger _logger;
      private ILogger Log { get { return _logger; } }

      public bool IsValidLogin( string userName, string password )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            var result = session.GetNamedQuery( "ValidateUser" )
               .SetString( "userName", userName )
               .SetString( "password", password )
               .List();

            return result.Count == 1;
         }
      }

      public bool ChangePassword( string userName, string oldPassword, string newPassword )
      {
         if (!IsValidLogin( userName, oldPassword ))
         {
            return false;
         }

         using (ISession session = NHibernateHelper.OpenSession())
         {
            using (ITransaction transaction = session.BeginTransaction())
            {
               session.CreateSQLQuery( "update users " +
                                          "set password = hashbytes('SHA1', cast(:newPass as varchar(4000))) " +
                                        "where userName = :userName" )
                  .SetString( "userName", userName )
                  .SetString( "newPass", newPassword )
                  .ExecuteUpdate();
               transaction.Commit();
            }
         }

         return true;
      }
   }
}
