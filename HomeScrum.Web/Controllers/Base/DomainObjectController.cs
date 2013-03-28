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
      // GET: /AcceptanceCriteriaStatuses/
      public virtual ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Details/5
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
      // GET: /AcceptanceCriteriaStatuses/Create
      public virtual ActionResult Create()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = new ModelT()
         };
         return View( viewModel );
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Create
      [HttpPost]
      public virtual ActionResult Create( EditViewModelT viewModel )
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
      // GET: /AcceptanceCriteriaStatuses/Edit/5
      public virtual ActionResult Edit( Guid id )
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = _repository.Get( id )
         };

         if (viewModel.DomainModel != null)
         {
            return View( viewModel );
         }

         return HttpNotFound();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( EditViewModelT viewModel )
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