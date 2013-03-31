using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   [Authorize]
   public class DomainObjectController<ModelT> : HomeScrumController
      where ModelT : new()
   {
      public DomainObjectController( IRepository<ModelT, Guid> repository, IValidator<ModelT> validator )
      {
         _repository = repository;
         _validator = validator;
      }

      private readonly IRepository<ModelT, Guid> _repository;
      protected IRepository<ModelT, Guid> Repository { get { return _repository; } }

      private readonly IValidator<ModelT> _validator;
      protected IValidator<ModelT> Validator { get { return _validator; } }

      //
      // GET: /ModelTs/
      public virtual ActionResult Index()
      {
         var items = _repository.GetAll();
         return View( items );
      }

      //
      // GET: /ModelTs/Details/5
      public virtual ActionResult Details( Guid id )
      {
         var model = _repository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( model );
      }

      //
      // GET: /ModelTs/Create
      public virtual ActionResult Create()
      {
         return View();
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      public virtual ActionResult Create( ModelT model )
      {
         Validate( model, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            _repository.Add( model );
            return RedirectToAction( () => this.Index() );
         }

         return View();
      }

      //
      // GET: /ModelTs/Edit/5
      public virtual ActionResult Edit( Guid id )
      {
         var model = _repository.Get( id );

         if (model != null)
         {
            return View( model );
         }

         return HttpNotFound();
      }

      //
      // POST: /ModelTs/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( ModelT model )
      {
         Validate( model, TransactionType.Update );

         if (ModelState.IsValid)
         {
            _repository.Update( model );

            return RedirectToAction( () => this.Index() );
         }
         else
         {
            return View();
         }
      }


      private void Validate( ModelT model, TransactionType transactionType )
      {
         if (!_validator.ModelIsValid( model, transactionType ))
         {
            foreach (var message in _validator.Messages)
            {
               ModelState.AddModelError( message.Key, message.Value );
            }
         }
      }
   }
}