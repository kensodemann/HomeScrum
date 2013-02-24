using System;

namespace HomeScrum.Data.Domain
{
   public class WorkItemType: BaseDataObject
   {
      public virtual char StatusCd { get; set; }
      public virtual char IsTask { get; set; }
      public virtual char IsPredefined { get; set; }
   }
}
