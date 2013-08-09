using HomeScrum.Data.Validation;
using NHibernate;
using Ninject;

namespace HomeScrum.Data.Domain
{
   public class SprintStatus : SystemDomainObject
   {
      public virtual bool IsOpenStatus { get; set; }

      #region Non-POCO
      public SprintStatus()
         : this( null ) { }

      [Inject]
      public SprintStatus( ISessionFactory sessionFactory )
         : base( sessionFactory )
      {
         _objectName = "Sprint Status";
      }

      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();

         if (_sessionFactory != null)
         {
            var session = _sessionFactory.GetCurrentSession();
            this.VerifyNameIsUnique( session );
         }
      }
      #endregion
   }
}
