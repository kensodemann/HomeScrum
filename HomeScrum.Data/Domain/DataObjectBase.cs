using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class DataObjectBase
   {
      public virtual Guid Id { get; set; }
      public virtual string Name { get; set; }
      public virtual string Description { get; set; }
   }
}
