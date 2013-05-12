using AutoMapper;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public abstract class ReadWriteController<ModelT, ViewModelT, EditorViewModelT>
      : ReadOnlyController<ModelT, ViewModelT>
      where EditorViewModelT : new()
   {
      public ReadWriteController( IRepository<ModelT> mainRepository, IValidator<ModelT> validator, IPropertyNameTranslator<ModelT, EditorViewModelT> translator )
         : base( mainRepository )
      {
         _validator = validator;
         _translator = translator;
      }

      private readonly IValidator<ModelT> _validator;
      private readonly IPropertyNameTranslator<ModelT, EditorViewModelT> _translator;
      protected IValidator<ModelT> Validator { get { return _validator; } }


      //
      // GET: /ModelTs/Create
      public virtual ActionResult Create()
      {
         var viewModel = new EditorViewModelT();

         PopulateSelectLists( viewModel );
         return View( viewModel );
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      public virtual ActionResult Create( EditorViewModelT viewModel, IPrincipal user )
      {
         var model = Mapper.Map<ModelT>( viewModel );
         Validate( model, TransactionType.Insert );

         if (ModelState.IsValid)
         {
            AddItem( model, user );
            return RedirectToAction( () => this.Index() );
         }

         PopulateSelectLists( viewModel );
         return View( viewModel );
      }


      //
      // GET: /ModelTs/Edit/Guid
      public virtual ActionResult Edit( Guid id )
      {
         var model = MainRepository.Get( id );

         if (model != null)
         {
            var viewModel = Mapper.Map<EditorViewModelT>( model );
            PopulateSelectLists( viewModel );
            return View( viewModel );
         }

         return HttpNotFound();
      }

      //
      // POST: /ModelTs/Edit/Guid
      [HttpPost]
      public virtual ActionResult Edit( EditorViewModelT viewModel, IPrincipal user )
      {
         var model = Mapper.Map<ModelT>( viewModel );
         Validate( model, TransactionType.Update );

         if (ModelState.IsValid)
         {
            UpdateItem( model, user );

            return RedirectToAction( () => this.Index() );
         }
         else
         {
            PopulateSelectLists( viewModel );
            return View( viewModel );
         }
      }


      protected virtual void PopulateSelectLists( EditorViewModelT viewModel ) { }


      protected virtual void AddItem( ModelT model, IPrincipal user )
      {
         MainRepository.Add( model );
      }


      protected virtual void UpdateItem( ModelT model, IPrincipal user )
      {
         MainRepository.Update( model );
      }


      protected virtual void PerformModelValidations( ModelT model )
      {
         ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType( () => model, typeof( ModelT ) );

         foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator( metadata, this.ControllerContext ).Validate( null ))
         {
            var viewModelPropertyName = _translator.TranslatedName( validationResult.MemberName );
            ModelState.AddModelError( viewModelPropertyName, validationResult.Message );
         }
      }

      protected void Validate( ModelT model, TransactionType transactionType )
      {
         PerformModelValidations( model );

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