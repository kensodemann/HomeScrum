using HomeScrum.Data.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   [ValidateWorkItemType]
   public class WorkItemType : SystemDataObject
   {
      [Display( Name = "WorkItemTypeIsTask", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsTask { get; set; }
   }
}
