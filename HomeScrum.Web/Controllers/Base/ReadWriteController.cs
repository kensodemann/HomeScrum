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
      where EditorViewModelT : DomainObjectViewModel, IEditorViewModel, new()
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
         var viewModel = new EditorViewModelT()
         {
            Mode = EditMode.Create
         };
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            UpdateNavigationStack( viewModel, callingController, callingAction, callingId );
            PopulateSelectLists( session, viewModel );
            transaction.Commit();
         }
         return View( viewModel );
      }

      //
      // POST: /ModelTs/Create
      [HttpPost]
      [ValidateInput( false )]
      public virtual ActionResult Create( EditorViewModelT viewModel, IPrincipal user )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               PeekNavigationData( viewModel );
               viewModel.Mode = EditMode.Create;
               if (ModelState.IsValid)
               {
                  var model = Mapper.Map<ModelT>( viewModel );
                  if (model.IsValidFor( Data.TransactionType.Insert ))
                  {
                     Save( session, model, user );
                     transaction.Commit();
                     return RedirectToCaller( viewModel );
                  }

                  TransferErrorMessages( model );
               }

               PopulateSelectLists( session, viewModel );
               transaction.Commit();
            }
            catch (Exception e)
            {
               transaction.Rollback();
               Log.Error( e, "Create POST error" );
            }
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
               viewModel.Mode = EditMode.Edit;
               UpdateNavigationStack( viewModel, callingController, callingAction, callingId );
               PopulateSelectLists( session, viewModel );
               transaction.Commit();
               return this.Request.IsAjaxRequest() ? PartialView( "_ModalEditor", viewModel ) : View( viewModel ) as ActionResult;
            }
            transaction.Commit();
         }

         return HttpNotFound();
      }

      //
      // POST: /ModelTs/Edit/Guid
      [HttpPost]
      [ValidateInput( false )]
      public virtual ActionResult Edit( EditorViewModelT viewModel, IPrincipal user )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               PeekNavigationData( viewModel );
               viewModel.Mode = EditMode.Edit;
               if (ModelState.IsValid)
               {
                  var model = Mapper.Map<ModelT>( viewModel );
                  if (model.IsValidFor( Data.TransactionType.Update ))
                  {
                     Update( session, model, user );
                     transaction.Commit();
                     return RedirectToCaller( viewModel );
                  }
                  TransferErrorMessages( model );
               }

               PopulateSelectLists( session, viewModel );
               transaction.Commit();
            }
            catch (Exception e)
            {
               transaction.Rollback();
               Log.Error( e, "Edit POST Error" );
            }
            return View( viewModel );
         }
      }

      private ActionResult RedirectToCaller( EditorViewModelT viewModel )
      {
         return RedirectToAction(
            viewModel.CallingAction ?? "index",
            viewModel.CallingController,
            viewModel.CallingId != Guid.Empty ? new { id = viewModel.CallingId.ToString() } : null );
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