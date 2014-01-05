using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Web.Models.Sprints
{
   interface IBurndownChart
   {
      String Name { get; }

      // Technically, these need to be not-null in order to display a burndown, 
      // but the sprint editor needs to be able to deal with NULL values
      DateTime? StartDate { get; }
      DateTime? EndDate { get; }

      IEnumerable<SprintCalendarEntryViewModel> Calendar { get; }
   }
}
