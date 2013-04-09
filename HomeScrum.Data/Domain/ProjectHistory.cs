using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class ProjectHistory : DomainObjectBase
   {
      public virtual Guid LastModifiedUserRid { get; set; }

      public virtual ProjectStatus ProjectStatus { get; set; }

      public virtual Guid ProjectRid { get; set; }

      public virtual DateTime HistoryTimestamp { get; set; }

      public virtual int SequenceNumber { get; set; }
   }
}
