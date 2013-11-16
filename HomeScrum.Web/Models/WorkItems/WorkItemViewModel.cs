using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.WorkItems
{
   public class WorkItemViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "WorkItemType", ResourceType = typeof( DisplayStrings ) )]
      public string WorkItemTypeName { get; set; }

      [Display( Name = "WorkItemStatus", ResourceType = typeof( DisplayStrings ) )]
      public string StatusName { get; set; }

      public bool IsComplete { get; set; }

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public string ProjectName { get; set; }

      [Display( Name = "CreatedByUser", ResourceType = typeof( DisplayStrings ) )]
      public string CreatedByUserName { get; set; }

      [Display( Name = "AssignedToUser", ResourceType = typeof( DisplayStrings ) )]
      public string AssignedToUserName { get; set; }

      [Display( Name = "ParentWorkItem", ResourceType = typeof( DisplayStrings ) )]
      public string ParentWorkItemName { get; set; }

      [Display( Name = "Sprint", ResourceType = typeof( DisplayStrings ) )]
      public string SprintName { get; set; }

      public IEnumerable<AcceptanceCriterionViewModel> AcceptanceCriteria { get; set; }

      public IEnumerable<WorkItemIndexViewModel> Tasks { get; set; }

      public int Points { get; set; }

      public int PointsRemaining { get; set; }
   }
}