using System;
using System.Linq;
using HomeScrum.Data.Domain;
using NHibernate;

namespace HomeScrum.Data.Queries
{
   public class ActiveSystemObjectsOrdered<ModelT> : AllSystemObjectsOrdered<ModelT>
      where ModelT : SystemDomainObject
   {
      public Guid SelectedId { get; set; }

      public override NHibernate.IQueryOver<ModelT, ModelT> GetQuery( ISession session )
      {
         return base.GetQuery( session )
            .Where( x => x.StatusCd == 'A' || x.Id == this.SelectedId );
      }

      public override IQueryable<ModelT> GetLinqQuery( ISession session )
      {
         return base.GetLinqQuery( session )
            .Where( x => x.StatusCd == 'A' || x.Id == this.SelectedId );
      }
   }
}
