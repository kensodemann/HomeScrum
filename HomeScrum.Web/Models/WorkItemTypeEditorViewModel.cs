using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class WorkItemTypeEditorViewModel : Base.SystemDomainObjectEditorViewModel
   {
      [Display( Name = "WorkItemTypeIsTask", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsTask { get; set; }
   }
}