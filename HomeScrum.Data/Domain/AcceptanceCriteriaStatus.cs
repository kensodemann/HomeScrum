using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriteriaStatus : SystemDataObject
   {
      [Display( Name = "AcceptanceCriteriaStatusIsAccepted", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsAccepted { get; set; }
   }
}
