using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   class WorkItem:DomainObjectBase
   {
      [Required]
      public virtual WorkItemStatus Status { get; set; }

      [Required]
      public virtual WorkItemType WorkItemType { get; set; }

      [Required]
      public virtual Project Project { get; set; }

      public virtual WorkItem ParentWorkItem { get; set; }

      //public virtual Sprint Sprint { get; set; }

      [Required]
      public virtual Guid LastModifiedUserRid { get; set; }

      [Required]
      public virtual Guid CreatedByUserRid { get; set; }

      public virtual Guid AssignedToUserRid { get; set; }
   }
}
