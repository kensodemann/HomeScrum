using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class WorkItemStatusEditorViewModel : Base.SystemDomainObjectEditorViewModel
   {
      [Display( Name = "WorkItemStatusIsOpenStatus", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsOpenStatus { get; set; }
   }
}