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
      // TODO: Add validation of UserId, but only if inserting...  hmmmm....
      private readonly IRepository<User, String> _repository;

      public UserValidator( IRepository<User, String> repository )
      {
         _repository = repository;
      }


      public bool ModelIsValid( User model, TransactionType forTransaction )
      {
         return true;
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { return new List<KeyValuePair<string, string>>(); }
      }
   }
}
