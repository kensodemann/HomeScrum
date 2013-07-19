using HomeScrum.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace HomeScrum.Data.Domain
{
   public class Project : DomainObjectBase
   {
      [Required]
      public virtual ProjectStatus Status { get; set; }

      public virtual Guid LastModifiedUserRid { get; set; }


      protected override void PerformModelValidations()
      {
         base.PerformModelValidations();

         using (var session = NHibernateHelper.OpenSession())
         {
            if (session.Query<Project>()
                   .Where( x => x.Id != this.Id && x.Name == this.Name )
                   .ToList().Count > 0)
            {
               _errorMessages.Add( "Name", String.Format( ErrorMessages.NameIsNotUnique, "Project", this.Name ) );
            }
         }
      }
   }
}
