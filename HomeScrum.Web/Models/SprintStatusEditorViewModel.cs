using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class SprintStatusEditorViewModel : Base.SystemDomainObjectEditorViewModel
   {
      [Display( Name = "SprintStatusIsOpenStatus", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsOpenStatus { get; set; }
   }
}