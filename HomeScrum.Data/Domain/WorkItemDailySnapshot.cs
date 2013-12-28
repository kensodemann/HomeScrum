using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class WorkItemDailySnapshot
   {
      public virtual WorkItem WorkItem { get; set; }
      public virtual DateTime HistoryDate { get; set; }
      public virtual int SortSequenceNumber { get; set; }
      public virtual int Points { get; set; }
      public virtual int PointsRemaining { get; set; }
   }
}
