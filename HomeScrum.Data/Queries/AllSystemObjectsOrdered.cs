using HomeScrum.Data.Domain;
using System.Linq;

namespace HomeScrum.Data.Queries
{
   public class AllSystemObjectsOrdered<ModelT> : AllDomainObjects<ModelT>
      where ModelT : SystemDomainObject
   {
      public override IQueryable<ModelT> GetQuery( NHibernate.ISession session )
      {
         return base.GetQuery( session )
            .OrderBy( x => x.SortSequence );
      }
   }
}
