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
   public abstract class ReadWriteController<ModelT, EditorViewModelT>
      : ReadOnlyController<ModelT>
      where ModelT : DomainObjectBase, HomeScrum.Data.Validation.IValidatable
      where EditorViewModelT : DomainObjectViewModel, new()
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
      public virtual ActionResult Create( string callingController = null, string callingAction = null, string callingId = null, string parentWorkItemId = null )
      {
         var viewModel = new EditorViewModelT();
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            UpdateNavigationStack( viewModel, callingAction, callingId );
            PopulateSelectLists( session, viewModel );
            transaction.Commit();
         }
         return View( viewModel );
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      [ValidateInput(false)]
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
                  return viewModel.CallingAction != null
                     ? RedirectToAction( viewModel.CallingAction, new { id = viewModel.CallingId.ToString() } )
                     : RedirectToAction( () => this.Index() );
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
      public virtual ActionResult Edit( Guid id, string callingController = null, string callingAction = null, string callingId = null )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var viewModel = GetEditorViewModel( session, id );

            if (viewModel != null)
            {
               UpdateNavigationStack( viewModel, callingAction, callingId );
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
      [ValidateInput(false)]
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
                  return viewModel.CallingAction != null
                     ? RedirectToAction( viewModel.CallingAction, new { id = viewModel.CallingId.ToString() } )
                     : RedirectToAction( () => this.Index() );
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

      protected virtual EditorViewModelT GetEditorViewModel( ISession session, Guid id )
      {
         var model = session.Get<ModelT>( id );
         return (model != null) ? Mapper.Map<EditorViewModelT>( model ) : null;
      }

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