using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer.Helpers;
using NHibernate;
using System;
using System.Collections.Generic;

namespace HomeScrum.Data.SqlServer
{
   public class DataObjectRepository<T> : IDataObjectRepository<T> where T : DataObjectBase
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

      public T Get( Guid id )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session.Get<T>( id );
         }
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
