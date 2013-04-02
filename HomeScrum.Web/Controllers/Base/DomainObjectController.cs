using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   [Authorize]
   public class DomainObjectController<ModelT> : HomeScrumController<ModelT>
      where ModelT : new()
   {
      public DomainObjectController( IRepository<ModelT, Guid> repository, IValidator<ModelT> validator )
         : base( repository )
      {
         _validator = validator;
      }

      private readonly IValidator<ModelT> _validator;
      protected IValidator<ModelT> Validator { get { return _validator; } }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      public virtual ActionResult Create( ModelT model )
      {
         Validate( model, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            Repository.Add( model );
            return RedirectToAction( () => this.Index() );
         }

         return View();
      }

      //
      // POST: /ModelTs/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( ModelT model )
      {
         Validate( model, TransactionType.Update );

         if (ModelState.IsValid)
         {
            Repository.Update( model );

            return RedirectToAction( () => this.Index() );
         }
         else
         {
            return View();
         }
      }


      private void Validate( ModelT model, TransactionType transactionType )
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