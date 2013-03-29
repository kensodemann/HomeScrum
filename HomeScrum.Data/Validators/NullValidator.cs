using HomeScrum.Data.Repositories;
using Ninject;
using System.Collections.Generic;

namespace HomeScrum.Data.Validators
{
   public class NullValidator<T, KeyT> : IValidator<T>
   {
      private readonly IRepository<T, KeyT> _repository;

      [Inject]
      public NullValidator( IRepository<T, KeyT> repository )
      {
         _repository = repository;
      }


      public bool ModelIsValid( T model )
      {
         return true;
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { return new List<KeyValuePair<string, string>>(); }
      }
   }
}
