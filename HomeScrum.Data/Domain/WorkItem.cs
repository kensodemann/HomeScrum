using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class WorkItem : DomainObjectBase
   {
      public WorkItem()
         : base( null ) { }

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
      public virtual User CreatedByUser { get; set; }

      public virtual User AssignedToUser { get; set; }

      public virtual IEnumerable<AcceptanceCriterion> AcceptanceCriteria { get; set; }

      //public virtual IEnumerable<WorkItem> Tasks { get; set; }
   }
}
