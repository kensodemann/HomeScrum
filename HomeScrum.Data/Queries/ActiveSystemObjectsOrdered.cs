using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Queries
{
   public class ActiveSystemObjectsOrdered<ModelT>:AllSystemObjectsOrdered<ModelT>
      where ModelT : SystemDomainObject
   {
      public Guid SelectedId { get; set; }

      public override NHibernate.IQueryOver<ModelT, ModelT> GetQuery( ISession session )
      {
         return base.GetQuery( session )
            .Where( x => x.StatusCd == 'A' || x.Id == this.SelectedId );
      }
   }
}
