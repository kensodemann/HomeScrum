using System;

namespace HomeScrum.Data.Domain
{
   public class WorkItemStatus : DataObjectBase
   {
      public virtual char StatusCd { get; set; }
      public virtual char IsOpenStatus { get; set; }
      public virtual char IsPredefined { get; set; }
   }
}
