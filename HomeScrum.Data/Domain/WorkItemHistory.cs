using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class WorkItemHistory
   {
      public virtual Guid Id { get; set; }

      public virtual DateTime HistoryTimestamp { get; set; }

      public virtual int SequenceNumber { get; set; }

      public virtual User LastModifiedUser { get; set; }

      public virtual WorkItem WorkItem { get; set; }
   }
}
