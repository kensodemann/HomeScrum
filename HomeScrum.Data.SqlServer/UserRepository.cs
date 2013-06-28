﻿using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer.Helpers;
using HomeScrum.Services;
using NHibernate;
using NHibernate.Criterion;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return session
               .CreateCriteria( typeof( User ) )
               .Add( Expression.Eq( "UserName", username ) )
               .UniqueResult() as User;
         }
      }
   }
}
