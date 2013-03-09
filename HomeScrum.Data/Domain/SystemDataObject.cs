using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SystemDataObject : DataObjectBase
   {
      private char StatusCd { get; set; }
      public virtual bool AllowUse
      {
         get { return StatusCd == 'A'; }
         set { StatusCd = value ? 'A' : 'I'; }
      }
      
      public virtual bool IsPredefined { get; set; }
   }
}
