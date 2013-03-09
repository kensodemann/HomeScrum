using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SprintStatus : SystemDataObject
   {
      public virtual bool IsOpenStatus { get; set; }
   }
}
