using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.Queries
{
   public class AllSystemObjectsOrdered<ModelT> : AllDomainObjects<ModelT>
      where ModelT : SystemDomainObject
   {
      public override NHibernate.IQueryOver<ModelT, ModelT> GetQuery( NHibernate.ISession session )
      {
         return base.GetQuery( session )
            .OrderBy( x => x.SortSequence ).Asc;
      }
   }
}
