using HomeScrum.Data.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class DomainObjectBase : IValidatable
   {
      public DomainObjectBase()
      {
         _errorMessages = new Dictionary<String, String>();
      }

      public virtual Guid Id { get; set; }

      [Required( ErrorMessageResourceName = "NameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      [MaxLength( 50, ErrorMessageResourceName = "NameMaxLength", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      public virtual string Name { get; set; }

      public virtual string Description { get; set; }


      // TODO: Look at making this an extension.
      public virtual bool IsValidFor( TransactionType transactionType )
      {
         _errorMessages.Clear();

         Validate( transactionType );

         return _errorMessages.Count == 0;
      }

      private void Validate( TransactionType transactionType )
      {
         PerformDataAnnotationValidations();
         PerformModelValidations();
      }

      protected virtual void PerformModelValidations() { }

      private void PerformDataAnnotationValidations()
      {
         var results = new List<ValidationResult>();
         var ctx = new ValidationContext( this );

         if (!Validator.TryValidateObject( this, ctx, results, true ))
         {
            foreach (var result in results)
            {
               _errorMessages.Add( result.MemberNames.First(), result.ErrorMessage );
            }
         }
      }

      protected IDictionary<String, String> _errorMessages;
      public virtual IDictionary<String, String> GetErrorMessages() { return _errorMessages; }
   }
}
