using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriteriaStatus : SystemDataObject
   {
      public virtual char IsAccepted { get; set; }
   }
}
