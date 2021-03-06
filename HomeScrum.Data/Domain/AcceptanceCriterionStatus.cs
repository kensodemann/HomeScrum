﻿using HomeScrum.Data.Validation;
using NHibernate;
using Ninject;

namespace HomeScrum.Data.Domain
{
   public class AcceptanceCriterionStatus : SystemDomainObject
   {
      public virtual AcceptanceCriterionStatusCategory Category { get; set; }

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
            var session = _sessionFactory.GetCurrentSession();
            this.VerifyNameIsUnique( session );
         }
      }
      #endregion
   }
}
