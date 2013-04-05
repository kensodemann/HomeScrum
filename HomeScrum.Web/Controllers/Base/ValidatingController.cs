using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Controllers.Base
{
   public class ValidatingController<ModelT> : HomeScrumController<ModelT>
   {
      public ValidatingController( IRepository<ModelT> repository, IValidator<ModelT> validator )
         : base( repository )
      {
         _validator = validator;
      }

      private readonly IValidator<ModelT> _validator;
      protected IValidator<ModelT> Validator { get { return _validator; } }


      protected void Validate( ModelT model, TransactionType transactionType )
      {
         if (!Validator.ModelIsValid( model, transactionType ))
         {
            foreach (var message in _validator.Messages)
            {
               ModelState.AddModelError( message.Key, message.Value );
            }
         }
      }
   }
}