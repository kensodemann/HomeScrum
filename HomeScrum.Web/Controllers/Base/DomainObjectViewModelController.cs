using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Controllers.Base
{
   public abstract class DomainObjectViewModelController<ModelT, ViewModelT> : DomainObjectController<ModelT>
      where ModelT : new()
   {
      public DomainObjectViewModelController( IRepository<ModelT, Guid> repository, IValidator<ModelT> validator )
         : base( repository, validator ) { }

      public override System.Web.Mvc.ActionResult Edit( Guid id )
      {
         var viewModel = CreateViewModel( Repository.Get( id ) );

         if (viewModel != null)
         {
            return View( viewModel );
         }

         return HttpNotFound();
      }

      public override System.Web.Mvc.ActionResult Edit( ModelT viewModel )
      {
         var model = CreateNewModel( viewModel );
         return base.Edit( model );
      }

      public override System.Web.Mvc.ActionResult Create( ModelT viewModel )
      {
         var model = CreateNewModel( viewModel );
         return base.Create( model );
      }

      protected abstract ViewModelT CreateViewModel( ModelT model );
      protected abstract ModelT CreateNewModel( ModelT viewModel );
   }
}