using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   [Authorize]
   public class DomainObjectController<ModelT, EditViewModelT> : HomeScrumController
      where ModelT : new()
      where EditViewModelT : IViewModel<ModelT>, new()
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
         var viewModel = CreateEditorViewModel( new ModelT() );
         return View( viewModel );
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      public ActionResult Create( EditViewModelT viewModel )
      {
         Validate( viewModel.DomainModel );

         if (ModelState.IsValid)
         {
            _repository.Add( viewModel.DomainModel );
            return RedirectToAction( () => this.Index() );
         }

         return View();
      }

      //
      // GET: /ModelTs/Edit/5
      public ActionResult Edit( Guid id )
      {
         var viewModel = CreateEditorViewModel( _repository.Get( id ) );

         if (viewModel.DomainModel != null)
         {
            return View( viewModel );
         }

         return HttpNotFound();
      }

      //
      // POST: /ModelTs/Edit/5
      [HttpPost]
      public ActionResult Edit( EditViewModelT viewModel )
      {
         Validate( viewModel.DomainModel );

         if (ModelState.IsValid)
         {
            _repository.Update( viewModel.DomainModel );

            return RedirectToAction( () => this.Index() );
         }
         else
         {
            return View();
         }
      }


      protected virtual EditViewModelT CreateEditorViewModel( ModelT model )
      {
         return new EditViewModelT()
         {
            DomainModel = model
         };
      }


      private void Validate( ModelT model )
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