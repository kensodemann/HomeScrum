using HomeScrum.Data.Validation;
using NHibernate;
using Ninject;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriterionStatus : SystemDomainObject
   {
      public virtual bool IsAccepted { get; set; }

      #region Non-POCO
      public AcceptanceCriterionStatus()
         : this( null ) { }

      [Inject]
      public AcceptanceCriterionStatus( ISessionFactory sessionFactory )
         : base( sessionFactory )
      {
         _objectName = "Acceptance Criterion Status";
      }

      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();

         if (_sessionFactory != null)
         {
            using (var session = _sessionFactory.OpenSession())
            {
               this.VerifyNameIsUnique( session );
            }
         }
      }
      #endregion
   }
}
