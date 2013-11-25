using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Sprints
{
   public class SprintWorkItemViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "WorkItemType", ResourceType = typeof( DisplayStrings ) )]
      public string WorkItemTypeName { get; set; }

      [Display( Name = "WorkItemStatus", ResourceType = typeof( DisplayStrings ) )]
      public string StatusName { get; set; }

      [Display(Name="IsInTargetSprint", ResourceType = typeof(DisplayStrings))]
      public bool IsInTargetSprint { get; set; }

      [Display( Name = "Points", ResourceType = typeof( DisplayStrings ) )]
      public int Points { get; set; }
      public int PointsRemaining { get; set; }
   }
}