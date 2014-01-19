using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Web.Models.Sprints;

namespace HomeScrum.Web.Models.Home
{
   public class SprintBurndown : IBurndownChart
   {
      public Guid Id { get; set; }

      public string Name { get; set; }

      public DateTime? StartDate { get; set; }

      public DateTime? EndDate { get; set; }

      public IEnumerable<SprintCalendarEntryViewModel> Calendar { get; set; }
   }
}