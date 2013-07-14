using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using NHibernate;
using NHibernate.Criterion;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Data.SqlServer
{
   public class UserRepository : Repository<User>, IUserRepository
   {
      [Inject]
      public UserRepository( ILogger logger ) : base( logger ) { }

      public User Get( string username )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            var query = session.CreateCriteria( typeof( User ) );
            query = query.Add( Expression.Eq( "UserName", username ) );
            return query.UniqueResult() as User;
         }
      }
   }
}
