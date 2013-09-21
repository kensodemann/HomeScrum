using System.ComponentModel;

namespace HomeScrum.Data.Domain
{
   public enum AcceptanceCriterionStatusCategory
   {
      [Description("Needs Verification")]
      NeedsVerification,

      [Description("Passed")]
      VerificationPassed,

      [Description("Failed")]
      VerificationFailed,

      Cancelled
   }
}
