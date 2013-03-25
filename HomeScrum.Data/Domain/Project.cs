using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class Project:DataObjectBase
   {
      public virtual Guid ProjectStatusRid { get; set; }
      public virtual ProjectStatus ProjectStatus { get; set; }

      public virtual string LastModifiedUserId { get; set; }
   }
}
