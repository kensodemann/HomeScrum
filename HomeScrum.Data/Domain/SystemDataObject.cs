using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SystemDataObject : DataObjectBase
   {
      public virtual char StatusCd { get; set; }
      public virtual bool IsPredefined { get; set; }
   }
}
