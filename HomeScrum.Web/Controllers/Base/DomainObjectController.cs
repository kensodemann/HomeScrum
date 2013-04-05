using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   [Authorize]
   public class DomainObjectController<ModelT> : ValidatingController<ModelT>
      where ModelT : new()
   {
      public DomainObjectController( IRepository<ModelT> repository, IValidator<ModelT> validator )
         : base( repository, validator ) { }

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
      // POST: /ModelTs/Edit/Guid
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
   }
}