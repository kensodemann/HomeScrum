using HomeScrum.Data.Repositories;
using Ninject;
using System;
using System.Collections.Generic;

namespace HomeScrum.Data.Validators
{
   public class NullValidator<T> : NullValidator<T, Guid>, IValidator<T>
   {
      [Inject]
      public NullValidator( IRepository<T> repository )
         : base( repository ) { }
   }
   
   public class NullValidator<T, KeyT> : IValidator<T>
   {
      private readonly IRepository<T, KeyT> _repository;

      [Inject]
      public NullValidator( IRepository<T, KeyT> repository )
      {
         _repository = repository;
      }


      public bool ModelIsValid( T model, TransactionType forTransaction )
      {
         return true;
      }

      public ICollection<KeyValuePair<string, string>> Messages
      {
         get { return new List<KeyValuePair<string, string>>(); }
      }
   }
}
