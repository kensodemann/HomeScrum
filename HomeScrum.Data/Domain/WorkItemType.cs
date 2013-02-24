using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   class WorkItemType: BaseDataObject
   {
      public virtual char StatusCd { get; set; }
      public virtual char IsTask { get; set; }
      public virtual char IsPredefined { get; set; }
   }
}
