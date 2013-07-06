using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriterionStatus : SystemDomainObject
   {
      public virtual bool IsAccepted { get; set; }
   }
}
