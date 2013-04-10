﻿using AutoMapper;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public abstract class ReadWriteController<ModelT> : ReadOnlyController<ModelT>
   {
      public ReadWriteController( IRepository<ModelT> mainRepository, IValidator<ModelT> validator )
         : base( mainRepository )
      {
         _validator = validator;
      }

      private readonly IValidator<ModelT> _validator;
      protected IValidator<ModelT> Validator { get { return _validator; } }


      //
      // GET: /ModelTs/Create
      public virtual ActionResult Create()
      {
         return View();
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      public virtual ActionResult Create( EditorViewModel  viewModel )
      {
         var model = Mapper.Map<ModelT>( viewModel );
         Validate( model, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            MainRepository.Add( model );
            return RedirectToAction( () => this.Index() );
         }

         return View();
      }

      //
      // GET: /ModelTs/Edit/Guid
      public virtual ActionResult Edit( Guid id )
      {
         var model = MainRepository.Get( id );

         if (model != null)
         {
            var viewModel = Mapper.Map<EditorViewModel>( model );
            return View( viewModel );
         }

         return HttpNotFound();
      }

      //
      // POST: /ModelTs/Edit/Guid
      [HttpPost]
      public virtual ActionResult Edit( ModelT model )
      {
         Validate( model, TransactionType.Update );

         if (ModelState.IsValid)
         {
            MainRepository.Update( model );

            return RedirectToAction( () => this.Index() );
         }
         else
         {
            return View();
         }
      }


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