using HomeScrum.Data.Validation;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriterionStatus : SystemDomainObject
   {
      public virtual bool IsAccepted { get; set; }

      #region Non-POCO
      public AcceptanceCriterionStatus()
      {
         _objectName = "Acceptance Criterion Status";
      }

      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();
         this.VerifyNameIsUnique();
      }
      #endregion
   }
}
