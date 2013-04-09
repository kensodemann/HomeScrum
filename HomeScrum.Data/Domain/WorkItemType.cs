using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class WorkItemType : SystemDomainObject
   {
      public virtual bool IsTask { get; set; }
   }
}
