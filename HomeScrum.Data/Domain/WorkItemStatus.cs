using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class WorkItemStatus : SystemDomainObject
   {
      public WorkItemStatus()
         : base() { }

      public WorkItemStatus( WorkItemStatus model )
         : base( model )
      {
         this.IsOpenStatus = model.IsOpenStatus;
      }

      [Display( Name = "WorkItemStatusIsOpenStatus", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsOpenStatus { get; set; }
   }
}
