using HomeScrum.Data.Domain;
using HomeScrum.Data.SqlServer.Helpers;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.SqlServer
{
   public class ProjectRepository : Repository<Project>
   {
      public override ICollection<Project> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( Project ) )
               .CreateAlias( "Status", "stat" )
               .AddOrder( Order.Asc( "stat.SortSequence" ) )
               .AddOrder( Order.Asc( "Name" ) )
               .List<Project>();
         }
      }
   }
}
