using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   public class UserValidator:IValidator<User>
   {
      private readonly IRepository<User, String> _repository;

      public UserValidator( IRepository<User, String> repository )
      {
         _repository = repository;
      }


      public bool ModelIsValid( User model )
      {
         return true;
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { return new List<KeyValuePair<string, string>>(); }
      }
   }
}
