using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class AcceptanceCriteriaStatusEditorViewModel : Base.SystemDomainObjectEditorViewModel
   {
      [Display( Name = "AcceptanceCriteriaStatusIsAccepted", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsAccepted { get; set; }
   }
}