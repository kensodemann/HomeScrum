using System;

namespace HomeScrum.Data.Domain
{
   public class WorkItemStatus : SystemDataObject
   {
      public virtual char IsOpenStatus { get; set; }
   }
}
