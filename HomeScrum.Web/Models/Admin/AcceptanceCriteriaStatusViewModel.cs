using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class AcceptanceCriteriaStatusViewModel : Base.SystemDomainObjectViewModel
   {
      [Display( Name = "AcceptanceCriteriaStatusIsAccepted", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsAccepted { get; set; }
   }
}