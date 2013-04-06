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
   public class SystemDataObjectValidator<DataObjectType> : IValidator<DataObjectType> where DataObjectType : SystemDomainObject
   {
      [Inject]
      public SystemDataObjectValidator( IRepository<DataObjectType> repository )
      {
         Repository = repository;
         Messages = new List<KeyValuePair<string, string>>();
      }

      protected IRepository<DataObjectType> Repository { get; private set; }
      protected virtual string ObjectName { get { return "System Data Object"; } }

      public virtual ICollection<KeyValuePair<string, string>> Messages { get; private set; }

      public virtual bool ModelIsValid( DataObjectType model, TransactionType forTransaction )
      {
         Messages.Clear();

         VerifyNameIsUnique( model );

         return Messages.Count == 0;
      }

      private void VerifyNameIsUnique( DataObjectType model )
      {
         if (ItemWithSameNameExists( model ))
         {
            AddMessage( "Name", ErrorMessages.NameIsNotUnique, model );
         }
      }

      private bool ItemWithSameNameExists( DataObjectType model )
      {
         return this.Repository.GetAll()
                   .FirstOrDefault( x => x.Name == model.Name && x.Id != model.Id ) != null;
      }

      private void AddMessage( string key, string message, DataObjectType model )
      {
         var newMessage = new KeyValuePair<string, string>( key, String.Format( message, ObjectName, model.Name ) );
         Messages.Add( newMessage );
      }
   }
}
