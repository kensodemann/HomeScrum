using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.Sprints;

namespace HomeScrum.Web.Models.Home
{
   public class Snapshot
   {
      public IEnumerable<DomainObjectViewModel> RecentActivity { get; set; }

      public IEnumerable<DomainObjectViewModel> NewestWorkItems { get; set; }

      public IEnumerable<DomainObjectViewModel> LatestSprints { get; set; }

      public IEnumerable<IBurndownChart> BurndownCharts { get; set; }
   }
}