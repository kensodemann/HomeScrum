using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class WorkItemStatus : SystemDomainObject
   {
     public virtual bool IsOpenStatus { get; set; }
   }
}
