using HomeScrum.Data.Validation;

namespace HomeScrum.Data.Domain
{
   public class ProjectStatus : SystemDomainObject
   {
      public virtual bool IsActive { get; set; }


      #region Non-POCO
      public ProjectStatus()
      {
         _objectName = "Project Status";
      }

      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();
         this.VerifyNameIsUnique();
      }
      #endregion
   }
}
