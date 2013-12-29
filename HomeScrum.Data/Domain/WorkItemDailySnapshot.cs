using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class WorkItemDailySnapshot
   {
      public virtual int SortSequenceNumber { get; set; }
      public virtual DateTime HistoryDate { get; set; }
      public virtual int Points { get; set; }
      public virtual int PointsRemaining { get; set; }
      public virtual WorkItem WorkItem { get; set; }

      public override bool Equals( object other )
      {
         if (this == other) return true;

         var item = other as WorkItemDailySnapshot;

         if (item == null || this.WorkItem != item.WorkItem || this.SortSequenceNumber != item.SortSequenceNumber)
         {
            return false;
         }

         return true;
      }

      public override int GetHashCode()
      {
         unchecked
         {
            int result;
            result = WorkItem.GetHashCode();
            result = 29 * result + SortSequenceNumber.GetHashCode();
            return result;
         }
      }
   }
}
