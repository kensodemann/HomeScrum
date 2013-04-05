using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer.Helpers;
using NHibernate;
using System;
using System.Collections.Generic;

namespace HomeScrum.Data.SqlServer
{
   public class Repository<T> : Repository<T, Guid>, IRepository<T> { }
   
   public class Repository<T, KeyT> : IRepository<T, KeyT>
   {
      public ICollection<T> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( T ).ToString() )
               .List<T>();
         }
      }

      public T Get( KeyT id )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return AreEqual(id,default( KeyT )) ? default( T ) : session.Get<T>( id );
         }
      }

      private bool AreEqual( KeyT x, KeyT y)
      {
         return EqualityComparer<KeyT>.Default.Equals(x, y);
      }

      public void Add( T dataObject )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            using (ITransaction transaction = session.BeginTransaction())
            {
               session.Save( dataObject );
               transaction.Commit();
            }
         }
      }

      public void Update( T dataObject )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            using (ITransaction transaction = session.BeginTransaction())
            {
               session.Update( dataObject );
               transaction.Commit();
            }
         }
      }

      public void Delete( T dataObject )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            using (ITransaction transaction = session.BeginTransaction())
            {
               session.Delete( dataObject );
               transaction.Commit();
            }
         }
      }
   }
}
