using HomeScrum.Data.Validation;

namespace HomeScrum.Data.Domain
{
   public class WorkItemType : SystemDomainObject
   {
      public virtual bool IsTask { get; set; }

      #region Non-POCO
      public WorkItemType()
      {
         _objectName = "Work Item Type";
      }

      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();
         this.VerifyNameIsUnique();
      }
      #endregion
   }
}
