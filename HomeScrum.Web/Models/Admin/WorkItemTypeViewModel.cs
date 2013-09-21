using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public class WorkItemTypeViewModel : Base.SystemDomainObjectViewModel
   {
      [Display( Name = "WorkItemTypeCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual String Category { get; set; }
   }
}