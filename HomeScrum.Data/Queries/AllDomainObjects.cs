using System.Linq;
using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;

namespace HomeScrum.Data.Queries
{
   public class AllDomainObjects<ModelT>
      where ModelT : DomainObjectBase
   {
      public virtual IQueryOver<ModelT,ModelT> GetQuery( ISession session )
      {
         var query = session.QueryOver<ModelT>();

         return query;
      }

      public virtual IQueryable<ModelT> GetLinqQuery( ISession session )
      {
         return session.Query<ModelT>();
      }
   }
}
