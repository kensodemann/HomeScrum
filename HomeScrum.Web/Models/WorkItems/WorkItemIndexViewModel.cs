using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.WorkItems
{
   public class WorkItemIndexViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "WorkItemType", ResourceType = typeof( DisplayStrings ) )]
      public string WorkItemTypeName { get; set; }

      [Display( Name = "WorkItemStatus", ResourceType = typeof( DisplayStrings ) )]
      public string StatusName { get; set; }

      //public bool IsComplete { get { return !IsOpenStatus; } }
      public bool IsComplete { get; set; }
   }
}