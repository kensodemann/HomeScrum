using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SprintCalendarEntry
   {
      public virtual Guid Id { get; set; }
      public virtual Sprint Sprint { get; set; }
      public virtual DateTime HistoryDate { get; set; }
      public virtual int PointsRemaining { get; set; }
   }
}
