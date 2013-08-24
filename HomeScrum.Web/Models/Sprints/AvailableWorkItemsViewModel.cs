using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Sprints
{
   public class AvailableWorkItemsViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "WorkItemType", ResourceType = typeof( DisplayStrings ) )]
      public string WorkItemTypeName { get; set; }

      [Display( Name = "Status", ResourceType = typeof( DisplayStrings ) )]
      public string StatusName { get; set; }

      public bool IsInTargetSprint { get; set; }

      public Guid TargetSprintRid { get; set; }
   }
}