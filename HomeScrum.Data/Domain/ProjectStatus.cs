using HomeScrum.Data.Validation;
using NHibernate;
using Ninject;

namespace HomeScrum.Data.Domain
{
   public class ProjectStatus : SystemDomainObject
   {
      public virtual bool IsActive { get; set; }


      #region Non-POCO
      public ProjectStatus()
         : this( null ) { }

      [Inject]
      public ProjectStatus( ISessionFactory sessionFactory )
         : base( sessionFactory )
      {
         _objectName = "Project Status";
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
