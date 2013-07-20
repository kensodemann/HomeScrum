using HomeScrum.Data.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class DomainObjectBase : ValidatableObject
   {
      public virtual Guid Id { get; set; }

      [Required( ErrorMessageResourceName = "NameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      [MaxLength( 50, ErrorMessageResourceName = "NameMaxLength", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      public virtual string Name { get; set; }

      public virtual string Description { get; set; }
   }
}
