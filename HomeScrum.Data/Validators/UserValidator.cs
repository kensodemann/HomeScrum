using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   public class UserValidator : IValidator<User>
   {
      private readonly IRepository<User> _repository;

      public UserValidator( IRepository<User> repository )
      {
         _repository = repository;
         Messages = new List<KeyValuePair<string, string>>();
      }


      public bool ModelIsValid( User model, TransactionType forTransaction )
      {
         if (!UsernameIsUnique( model ))
         {
            AddMessage( "UserName", ErrorMessages.UsernameIsNotUnique, model );
            return false;
         }

         return true;
      }

      private bool UsernameIsUnique( User model )
      {
         var user = _repository
            .GetAll()
            .FirstOrDefault( x => x.UserName == model.UserName && x.Id != model.Id );

         return (user == null);
      }

      private void AddMessage( string key, string message, User user )
      {
         var newMessage = new KeyValuePair<string, string>( key, String.Format( message, user.UserName ) );
         Messages.Add( newMessage );
      }

      public ICollection<KeyValuePair<string, string>> Messages { get; private set; }
   }
}
