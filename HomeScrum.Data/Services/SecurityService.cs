using HomeScrum.Common.Utility;
using HomeScrum.Data.Repositories;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;
using System.Diagnostics;

namespace HomeScrum.Data.Services
{
   public class SecurityService : ISecurityService
   {
      [Inject]
      public SecurityService( ILogger logger, ISessionFactory sessionFactory )
      {
         _logger = logger;
         _sessionFactory = sessionFactory;
      }

      private readonly ILogger _logger;
      private ILogger Log { get { return _logger; } }

      private readonly ISessionFactory _sessionFactory;

      public bool IsValidLogin( string userName, string password )
      {
         using (ISession session = _sessionFactory.OpenSession())
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

         var session = _sessionFactory.GetCurrentSession();
         Debug.Assert( session.Transaction != null && session.Transaction.IsActive );

         session.CreateSQLQuery( "update users " +
                                    "set password = hashbytes('SHA1', cast(:newPass as varchar(4000))) " +
                                  "where userName = :userName" )
            .SetString( "userName", userName )
            .SetString( "newPass", newPassword )
            .ExecuteUpdate();

         return true;
      }
   }
}
