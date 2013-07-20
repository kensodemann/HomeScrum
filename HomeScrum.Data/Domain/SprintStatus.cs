using HomeScrum.Data.Validation;

namespace HomeScrum.Data.Domain
{
   public class SprintStatus : SystemDomainObject
   {
     public virtual bool IsOpenStatus { get; set; }

      #region Non-POCO
      public SprintStatus()
      {
         _objectName = "Sprint Status";
      }

      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();
         this.VerifyNameIsUnique();
      }
      #endregion
   }
}
