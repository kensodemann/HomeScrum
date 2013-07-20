using HomeScrum.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using HomeScrum.Data.Validation;

namespace HomeScrum.Data.Domain
{
   public class Project : DomainObjectBase
   {
      public Project()
      {
         _objectName = "Project";
      }

      [Required]
      public virtual ProjectStatus Status { get; set; }

      public virtual Guid LastModifiedUserRid { get; set; }


      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();
         this.VerifyNameIsUnique();
      }
   }
}
