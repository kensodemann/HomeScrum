using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class ProjectHistory : Project
   {
      public virtual Guid ProjectRid { get; set; }

      public virtual DateTime HistoryTimestamp { get; set; }

      public virtual int SequenceNumber { get; set; }
   }
}
