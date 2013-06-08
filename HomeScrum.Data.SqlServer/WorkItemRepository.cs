﻿using HomeScrum.Data.Domain;
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
   public class WorkItemRepository : Repository<WorkItem>
   {
      public override ICollection<WorkItem> GetAll()
      {
         using (ISession session = NHibernateHelper.OpenSession())
         {
            return session
               .CreateCriteria( typeof( WorkItem ) )
               .CreateAlias( "Status", "stat" )
               .CreateAlias( "WorkItemType", "wit" )
               .AddOrder( Order.Asc( "wit.SortSequence" ) )
               .AddOrder( Order.Asc( "stat.SortSequence" ) )
               .AddOrder( Order.Asc( "Name" ) )
               .List<WorkItem>();
         }
      }
   }
}