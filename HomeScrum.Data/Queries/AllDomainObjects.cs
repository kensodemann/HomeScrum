using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;
using System.Linq;

namespace HomeScrum.Data.Queries
{
   public class AllDomainObjects<ModelT>
      where ModelT : DomainObjectBase
   {
      public virtual IQueryable<ModelT> GetQuery( ISession session )
      {
         return session.Query<ModelT>();
      }
   }
}
