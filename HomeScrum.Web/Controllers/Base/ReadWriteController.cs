using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using NHibernate;
using Ninject.Extensions.Logging;
using System;
using System.Security.Principal;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public abstract class ReadWriteController<ModelT, ViewModelT, EditorViewModelT>
      : ReadOnlyController<ModelT, ViewModelT>
      where ModelT : DomainObjectBase
      where ViewModelT : DomainObjectViewModel
      where EditorViewModelT : new()
   {
      public ReadWriteController( IPropertyNameTranslator<ModelT, EditorViewModelT> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory )
      {
         _translator = translator;
      }

      private readonly IPropertyNameTranslator<ModelT, EditorViewModelT> _translator;
      protected IPropertyNameTranslator<ModelT, EditorViewModelT> PropertyNameTranslator { get { return _translator; } }

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
         using (var session = SessionFactory.OpenSession())
         {
            var model = session.Get<ModelT>( id );

            if (model != null)
            {
               var viewModel = Mapper.Map<EditorViewModelT>( model );
               PopulateSelectLists( viewModel );
               return View( viewModel );
            }

            return HttpNotFound();
         }
      }

      //
      // POST: /ModelTs/Edit/Guid
      [HttpPost]
      public virtual ActionResult Edit( EditorViewModelT viewModel, IPrincipal user )
      {
         var model = Mapper.Map<ModelT>( viewModel );

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
         using (var session = SessionFactory.OpenSession())
         {
            using (var transaction = session.BeginTransaction())
            {
               session.Save( model );
               transaction.Commit();
            }
         }
      }


      protected virtual void UpdateItem( ModelT model, IPrincipal user )
      {
         using (var session = SessionFactory.OpenSession())
         {
            using (var transaction = session.BeginTransaction())
            {
               session.Update( model );
               transaction.Commit();
            }
         }
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
   }
}