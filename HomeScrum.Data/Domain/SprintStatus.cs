using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class SprintStatus : SystemDomainObject
   {
     public virtual bool IsOpenStatus { get; set; }
   }
}
