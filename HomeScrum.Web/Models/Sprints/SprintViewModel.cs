using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Sprints
{
   public class SprintViewModel : Base.DomainObjectViewModel, IBurndownChart
   {
      [Display( Name = "SprintStatus", ResourceType = typeof( DisplayStrings ) )]
      public string StatusName { get; set; }

      public bool IsComplete { get; set; }
      public bool CanAddBacklog { get; set; }
      public bool CanAddTasks { get; set; }

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public string ProjectName { get; set; }

      [Display( Name = "StartDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime? StartDate { get; set; }

      [Display( Name = "EndDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime? EndDate { get; set; }

      [Display( Name = "Capacity", ResourceType = typeof( DisplayStrings ) )]
      public int Capacity { get; set; }

      [Display( Name = "PointsScheduled", ResourceType = typeof( DisplayStrings ) )]
      public int TotalPoints { get; set; }

      public IEnumerable<SprintCalendarEntryViewModel> Calendar { get; set; }

      public IEnumerable<SprintWorkItemViewModel> BacklogItems { get; set; }

      public IEnumerable<SprintWorkItemViewModel> Tasks { get; set; }
   }
}