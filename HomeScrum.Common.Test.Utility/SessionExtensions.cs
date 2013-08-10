using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.Test.Utility
{
   public static class SessionExtensions
   {
      public static bool DataAlreadyLoaded<T>( this ISession session )
      {
         using (var transaction = session.BeginTransaction())
         {
            var count = session.Query<T>().Count();
            transaction.Commit();

            return count > 0;
         }
      }

      public static void LoadIntoDatabase<T>( this ISession session, IEnumerable<T> data )
      {
         using (var transaction = session.BeginTransaction())
         {
            foreach (var item in data)
               session.Save( item );
            transaction.Commit();
         }
         session.Clear();
      }
   }
}
