using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class Project : DomainObjectBase
   {
      [Required]
      public virtual ProjectStatus Status { get; set; }

      public virtual Guid LastModifiedUserRid { get; set; }
   }
}
