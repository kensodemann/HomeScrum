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
      // TODO: Add validation of UserName
      private readonly IRepository<User, Guid> _repository;

      public UserValidator( IRepository<User, Guid> repository )
      {
         _repository = repository;
      }


      public bool ModelIsValid( User model, TransactionType forTransaction )
      {
         var user = _repository
            .GetAll()
            .FirstOrDefault( x => x.UserName == model.UserName && x.Id != model.Id );

         return (user == null);
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { return new List<KeyValuePair<string, string>>(); }
      }
   }
}
