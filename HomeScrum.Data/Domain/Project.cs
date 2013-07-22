using HomeScrum.Data.Validation;
using NHibernate;
using Ninject;
using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Domain
{
   public class Project : DomainObjectBase
   {
      [Required]
      public virtual ProjectStatus Status { get; set; }

      public virtual Guid LastModifiedUserRid { get; set; }


      #region Non-POCO stuff
      public Project()
         : this( null ) { }

      [Inject]
      public Project( ISessionFactory sessionFactory )
         : base( sessionFactory )
      {
         _objectName = "Project";
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
