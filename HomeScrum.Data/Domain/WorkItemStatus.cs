using System;

namespace HomeScrum.Data.Domain
{
   public class WorkItemStatus : SystemDataObject
   {
      public virtual bool IsOpenStatus { get; set; }
   }
}
