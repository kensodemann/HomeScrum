using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   [Authorize]
   public class DataObjectBaseController<T, KeyT> : Controller
   {
      public DataObjectBaseController( IRepository<T, KeyT> repository, IValidator<T> validator )
      {
         _repository = repository;
         _validator = validator;
      }

      private readonly IRepository<T, KeyT> _repository;
      public IRepository<T, KeyT> Repository { get { return _repository; } }

      private readonly IValidator<T> _validator;
      public IValidator<T> Validator { get { return _validator; } }

      //
      // GET: /AcceptanceCriteriaStatuses/
      public virtual ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Details/5
      public virtual ActionResult Details( KeyT id )
      {
         var model = _repository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( model );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Create
      public virtual ActionResult Create()
      {
         return View();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Create
      [HttpPost]
      public virtual ActionResult Create( T model )
      {
         Validate( model );

         if (ModelState.IsValid)
         {
            _repository.Add( model );
            return RedirectToAction( "Index" );
         }

         return View();
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Edit/5
      public virtual ActionResult Edit( KeyT id )
      {
         var model = _repository.Get( id );
         if (model != null)
         {
            return View( model );
         }

         return HttpNotFound();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( T model )
      {
         Validate( model );

         if (ModelState.IsValid)
         {
            _repository.Update( model );

            return RedirectToAction( "Index" );
         }
         else
         {
            return View();
         }
      }


      private void Validate( T model )
      {
         if (!_validator.ModelIsValid( model ))
         {
            foreach (var message in _validator.Messages)
            {
               ModelState.AddModelError( message.Key, message.Value );
            }
         }
      }
   }
}