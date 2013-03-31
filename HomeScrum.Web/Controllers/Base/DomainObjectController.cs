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
      public IRepository<ModelT, Guid> Repository { get { return _repository; } }

      private readonly IValidator<ModelT> _validator;
      public IValidator<ModelT> Validator { get { return _validator; } }

      //
      // GET: /ModelTs/
      public ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /ModelTs/Details/5
      public ActionResult Details( Guid id )
      {
         var model = _repository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( model );
      }

      //
      // GET: /ModelTa/Create
      public ActionResult Create()
      {
         return View();
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      public ActionResult Create( ModelT model )
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
      public ActionResult Edit( Guid id )
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
      public ActionResult Edit( ModelT model )
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