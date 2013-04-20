using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class WorkItemTypeEditorViewModel : Base.SystemDomainObjectViewModel
   {
      [Display( Name = "WorkItemTypeIsTask", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsTask { get; set; }
   }
}