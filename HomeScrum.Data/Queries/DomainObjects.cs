using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Queries
{
   public class DomainObjects<ModelT>
      where ModelT : DomainObjectBase
   {
      public virtual IQueryOver<ModelT,ModelT> GetQuery( ISession session )
      {
         var query = session.QueryOver<ModelT>();

         return query;
      }
   }
}
