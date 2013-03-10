using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class WorkItemType : SystemDataObject
   {
      [Display( Name = "Is Task" )]
      public virtual bool IsTask { get; set; }
   }
}
