using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace HomeScrum.Data.Queries
{
   public interface IDomainObjectQuery
   {
      ICriteria GetQuery( ISession session );
   }
}
