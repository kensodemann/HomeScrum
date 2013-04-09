using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class ProjectStatus : SystemDomainObject
   {
      public virtual bool IsActive { get; set; }
   }
}
