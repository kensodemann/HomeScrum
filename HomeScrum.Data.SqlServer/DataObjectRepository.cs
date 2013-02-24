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
            return new List<DataObjectBase>();
            return session
               .CreateCriteria( typeof( DataObjectBase ).ToString() )
               .List<DataObjectBase>();
         }
      }

      public DataObjectBase Get( Guid id )
      {
         throw new NotImplementedException();
      }

      public void Add( DataObjectBase dataObject )
      {
         throw new NotImplementedException();
      }

      public void Update( DataObjectBase dataObject )
      {
         throw new NotImplementedException();
      }

      public void Delete( Guid id )
      {
         throw new NotImplementedException();
      }
   }
}
