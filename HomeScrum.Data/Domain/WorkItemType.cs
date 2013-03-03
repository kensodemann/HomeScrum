using System;

namespace HomeScrum.Data.Domain
{
   public class WorkItemType: DataObjectBase
   {
      public virtual char StatusCd { get; set; }
      public virtual char IsTask { get; set; }
      public virtual char IsPredefined { get; set; }
   }
}
