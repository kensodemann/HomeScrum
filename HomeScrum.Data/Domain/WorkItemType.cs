using HomeScrum.Data.Validation;
using NHibernate;
using Ninject;

namespace HomeScrum.Data.Domain
{
   public class WorkItemType : SystemDomainObject
   {
      public virtual bool IsTask { get; set; }

      #region Non-POCO
      public WorkItemType() : this( null ) { }

      [Inject]
      public WorkItemType( ISessionFactory sessionFactory )
         : base( sessionFactory )
      {
         _objectName = "Work Item Type";
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
