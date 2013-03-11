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
         this.Repository.GetAll();

         return ValidationResult.Success;
      }
   }
}
