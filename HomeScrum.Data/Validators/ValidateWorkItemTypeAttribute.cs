using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   [AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
   public sealed class ValidateWorkItemTypeAttribute : ValidationAttribute
   {
      [Inject]
      public IDataObjectRepository<WorkItemType> Repository { get; set; }

      protected override ValidationResult IsValid( object value, ValidationContext validationContext )
      {
         var workItemType = value as WorkItemType;
         if (workItemType != null)
         {
            var typeWithSameName = this.Repository.GetAll().FirstOrDefault( x => x.Name == workItemType.Name );
            if (typeWithSameName != null)
            {
               return new ValidationResult( this.ErrorMessage );
            }
         }

         return ValidationResult.Success;
      }
   }
}
