using System;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using NHibernate;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.Controllers.Base
{
   public abstract class ReadWriteController<ModelT, ViewModelT, EditorViewModelT>
      : ReadOnlyController<ModelT, ViewModelT>
      where ModelT : DomainObjectBase, HomeScrum.Data.Validation.IValidatable
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
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            PopulateSelectLists( session, viewModel );
            transaction.Commit();
         }
         return View( viewModel );
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      public virtual ActionResult Create( EditorViewModelT viewModel, IPrincipal user )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            if (ModelState.IsValid)
            {
               var model = Mapper.Map<ModelT>( viewModel );
               if (model.IsValidFor( Data.TransactionType.Insert ))
               {
                  Save( session, model, user );
                  transaction.Commit();
                  return RedirectToAction( () => this.Index() );
               }
               TransferErrorMessages( model );
            }

            PopulateSelectLists( session, viewModel );
            transaction.Commit();
            return View( viewModel );
         }
      }

      //
      // GET: /ModelTs/Edit/Guid
      public virtual ActionResult Edit( Guid id )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var model = session.Get<ModelT>( id );

            if (model != null)
            {
               var viewModel = Mapper.Map<EditorViewModelT>( model );
               PopulateSelectLists( session, viewModel );
               transaction.Commit();
               return View( viewModel );
            }
            transaction.Commit();
         }

         return HttpNotFound();
      }

      //
      // POST: /ModelTs/Edit/Guid
      [HttpPost]
      public virtual ActionResult Edit( EditorViewModelT viewModel, IPrincipal user )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            if (ModelState.IsValid)
            {
               var model = Mapper.Map<ModelT>( viewModel );
               if (model.IsValidFor( Data.TransactionType.Update ))
               {
                  Update( session, model, user );
                  transaction.Commit();
                  return RedirectToAction( () => this.Index() );
               }
               TransferErrorMessages( model );
            }

            PopulateSelectLists( session, viewModel );
            transaction.Commit();
            return View( viewModel );
         }
      }

      private void TransferErrorMessages( ModelT model )
      {
         foreach (var message in model.GetErrorMessages())
         {
            var viewModelPropertyName = PropertyNameTranslator.TranslatedName( message.Key );
            ModelState.AddModelError( viewModelPropertyName, message.Value );
         }
      }

      protected virtual void PopulateSelectLists( ISession session, EditorViewModelT viewModel ) { }


      protected virtual void Save( ISession session, ModelT model, IPrincipal user )
      {
         session.Save( model );
      }


      protected virtual void Update( ISession session, ModelT model, IPrincipal user )
      {
         session.Update( model );
      }
   }
}