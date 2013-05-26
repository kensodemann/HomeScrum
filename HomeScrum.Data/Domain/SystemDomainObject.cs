using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SystemDomainObject : DomainObjectBase
   {
      public virtual char StatusCd { get; set; }

      public virtual bool IsPredefined { get; set; }

      public virtual int SortSequence { get; set; }
   }
}
