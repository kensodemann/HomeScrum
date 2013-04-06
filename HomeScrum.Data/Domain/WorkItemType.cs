using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class WorkItemType : SystemDomainObject
   {
      public WorkItemType()
         : base() { }

      public WorkItemType( WorkItemType model )
         : base( model )
      {
         this.IsTask = model.IsTask;
      }

      [Display( Name = "WorkItemTypeIsTask", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsTask { get; set; }
   }
}
