using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer.Helpers;
using NHibernate;
using System;
using System.Collections.Generic;

namespace HomeScrum.Data.SqlServer
{
   public class DataObjectRepository<DataObjectBase> : IDataObjectRepository<DataObjectBase> where DataObjectBase : BaseDataObject
   {
      public ICollection<DataObjectBase> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( DataObjectBase ).ToString() )
               .List<DataObjectBase>();
         }
      }

      public DataObjectBase Get( Guid id )
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session.Get<DataObjectBase>( id );
         }
      }

      public void Add( DataObjectBase dataObject )
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

      public void Update( DataObjectBase dataObject )
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

      public void Delete( DataObjectBase dataObject )
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
