using System;
using System.ComponentModel.DataAnnotations;
using HomeScrum.Data.Validation;

namespace HomeScrum.Data.Domain
{
   public class WorkItemStatus : SystemDomainObject
   {
     public virtual bool IsOpenStatus { get; set; }

      #region Non-POCO
      public WorkItemStatus()
      {
         _objectName = "Work Item Status";
      }

      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();
         this.VerifyNameIsUnique();
      }
      #endregion
   }
}
