using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Security.Principal;

namespace HomeScrum.Web.Extensions
{
   public static class IIdentityExtensions
   {
      public static Guid GetUserId( this IIdentity identity, ISession session )
      {
         return session.Query<User>()
            .Single( x => x.UserName == identity.Name ).Id;
      }
   }
}