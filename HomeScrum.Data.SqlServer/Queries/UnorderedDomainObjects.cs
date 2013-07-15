using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.SqlServer.Queries
{
   public class DomainObjects<ModelT>
      where ModelT : DomainObjectBase
   {
      public IQueryOver GetQuery( ISession session )
      {
         var query = session.QueryOver<ModelT>();

         return query;
      }
   }
}
