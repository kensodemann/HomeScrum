using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class SprintStatusViewModel : Base.SystemDomainObjectViewModel
   {
      [Display( Name = "SprintStatusCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual string Category { get; set; }

      [Display( Name = "SprintStatusAllowNewBacklogItems", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool CanAddBacklogItems { get; set; }

      [Display( Name = "SprintStatusAllowNewTaskListItems", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool CanAddTaskListItems { get; set; }
   }
}