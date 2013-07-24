using HomeScrum.Data.Domain;
using NHibernate;
using System;
using System.Linq;

namespace HomeScrum.Data.Queries
{
   public class ActiveSystemObjectsOrdered<ModelT> : AllSystemObjectsOrdered<ModelT>
      where ModelT : SystemDomainObject
   {
      public Guid SelectedId { get; set; }

      public override IQueryable<ModelT> GetQuery( ISession session )
      {
         return base.GetQuery( session )
            .Where( x => x.StatusCd == 'A' || x.Id == this.SelectedId );
      }
   }
}
