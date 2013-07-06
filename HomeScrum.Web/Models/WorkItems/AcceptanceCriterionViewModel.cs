using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.WorkItems
{
   public class AcceptanceCriterionViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "AcceptanceCriteriaIsAccepted", ResourceType = typeof( DisplayStrings ) )]
      public bool IsAccepted { get; set; }
   }
}