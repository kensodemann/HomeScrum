using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriteriaStatus:BaseDataObject
   {
      public virtual char StatusCd { get; set; }
      public virtual char IsAccepted { get; set; }
      public virtual char IsPredefined { get; set; }
   }
}
