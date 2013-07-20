using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HomeScrum.Data.Validation
{
   public class ValidatableObject : IValidatable
   {
      public ValidatableObject()
      {
         _errorMessages = new Dictionary<String, String>();
         _objectName = "not specified";
      }


      protected string _objectName;
      public virtual string GetObjectName() { return _objectName; }


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
      public virtual void AddErrorMessage( string key, string message )
      {
         _errorMessages.Add( key, message );
      }
   }
}
