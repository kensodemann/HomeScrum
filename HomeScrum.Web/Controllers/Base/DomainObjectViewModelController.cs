using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public abstract class DomainObjectViewModelController<ModelT, ViewModelT> : DomainObjectController<ModelT>
      where ModelT : new()
   {
      public DomainObjectViewModelController( IRepository<ModelT, Guid> repository, IValidator<ModelT> validator )
         : base( repository, validator ) { }

      public override ActionResult Edit( Guid id )
      {
         var model = Repository.Get( id );
         
         if (model != null)
         {
            var viewModel = CreateViewModel( model );
            return View( viewModel );
         }

         return HttpNotFound();
      }

      protected abstract ViewModelT CreateViewModel( ModelT model );
   }
}