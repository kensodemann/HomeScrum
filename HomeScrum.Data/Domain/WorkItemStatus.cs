using System;
using System.ComponentModel.DataAnnotations;
using HomeScrum.Data.Validation;
using Ninject;
using NHibernate;

namespace HomeScrum.Data.Domain
{
   public class WorkItemStatus : SystemDomainObject
   {
      public virtual bool IsOpenStatus { get; set; }

      #region Non-POCO
      public WorkItemStatus()
         : this( null ) { }

      [Inject]
      public WorkItemStatus( ISessionFactory sessionFactory )
         : base( sessionFactory )
      {
         _objectName = "Work Item Status";
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
