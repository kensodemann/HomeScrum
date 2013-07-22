using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriterion : DomainObjectBase
   {
      public AcceptanceCriterion()
         : base( null ) { }

      [Required]
      public virtual AcceptanceCriterionStatus Status { get; set; }

      [Required]
      public virtual WorkItem WorkItem { get; set; }
   }
}
