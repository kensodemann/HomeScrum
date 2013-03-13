using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   public class SystemDataObjectValidator<DataObjectType> : IValidator<DataObjectType> where DataObjectType : SystemDataObject
   {
      [Inject]
      public SystemDataObjectValidator( IDataObjectRepository<DataObjectType> repository )
      {
         Repository = repository;
      }

      public IDataObjectRepository<DataObjectType> Repository { get; private set; }

      public bool ModelIsValid( DataObjectType model )
      {
         return !ItemWithSameNameExists( model );
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { throw new NotImplementedException(); }
      }
      //protected override ValidationResult IsValid( object value, ValidationContext validationContext )
      //{
      //   if (WorkItemTypeWithSameNameExists( value ))
      //   {
      //      return new ValidationResult( GetErrorMessage() );
      //   }

      //   return ValidationResult.Success;
      //}

      private bool ItemWithSameNameExists( DataObjectType model )
      {
         return this.Repository.GetAll()
                   .FirstOrDefault( x => x.Name == model.Name && x.Id != model.Id ) != null;
      }

      //private string GetErrorMessage()
      //{
      //   if (!String.IsNullOrWhiteSpace( this.ErrorMessageResourceName ) && this.ErrorMessageResourceType != null)
      //   {
      //      return ErrorMessageResourceType.InvokeMember( this.ErrorMessageResourceName,
      //         System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetProperty,
      //         null, null, null ).ToString();
      //   }
      //   return this.ErrorMessage;
      //}
   }
}
