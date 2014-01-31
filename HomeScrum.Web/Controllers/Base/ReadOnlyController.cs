using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Base;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.Controllers.Base
{
   /// <summary>
   /// The ReadOnlyController is the base class for any contoller in the system that only supports
   /// the GET operations.  The only action for this type of controller is Index.
   /// </summary>
   /// <typeparam name="ModelT">The Domain Model Type for the main data</typeparam>
   [Authorize]
   public abstract class ReadOnlyController<ModelT, ViewModelT> : Controller
      where ModelT : DomainObjectBase
      where ViewModelT : DomainObjectViewModel
   {
      private readonly ISessionFactory _sessionFactory;
      protected ISessionFactory SessionFactory { get { return _sessionFactory; } }

      private readonly ILogger _logger;
      protected ILogger Log { get { return _logger; } }

      public ReadOnlyController( ILogger logger, ISessionFactory sessionFactory )
      {
         _logger = logger;
         _sessionFactory = sessionFactory;
      }

      //
      // GET: /ModelTs/
      public virtual ActionResult Index()
      {
         Log.Debug( "Index()" );

         var session = SessionFactory.GetCurrentSession();
         var query = session.Query<ModelT>().SelectDomainObjectViewModels<ModelT>();

         return IndexView( query );
      }

      protected ActionResult IndexView<T>( IQueryable<T> query )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var items = query.ToList();
            transaction.Commit();
            ClearNavigationStack();
            return View( items );
         }
      }

      //
      // GET: /ModelTs/Display/Guid
      public virtual ActionResult Details( Guid id, string callingController = null, string callingAction = null, string callingId = null )
      {
         ViewModelT viewModel;
         Log.Debug( "Display({0}", id.ToString() );

         var session = SessionFactory.GetCurrentSession();

         using (var transaction = session.BeginTransaction())
         {
            viewModel = GetViewModel( session, id );
            transaction.Commit();
         }

         if (viewModel == null)
         {
            return HttpNotFound();
         }

         UpdateNavigationStack( viewModel, callingController, callingAction, callingId );

         return View( viewModel );
      }


      protected virtual ViewModelT GetViewModel( ISession session, Guid id )
      {
         var model = session.Get<ModelT>( id );
         return (model != null) ? Mapper.Map<ViewModelT>( model ) : null;
      }


      /// <summary>
      /// This method is intended to be called from the GET actions.  All NavigationStack maintenence is done from the GET
      /// because the user will not go through a POST, which is where a pop() would logically ding, in the case where they
      /// nav back or where they use the Cancel link.
      /// </summary>
      /// <param name="viewModel">Receives the new top calling information</param>
      /// <param name="callingController">Calling controller, null in case of cancel</param>
      /// <param name="callingAction">Calling action, null in case of cancel</param>
      /// <param name="callingId">Calling Id, null in case of cancel</param>
      protected void UpdateNavigationStack( ViewModelBase viewModel, string callingController, string callingAction, string callingId )
      {
         if (callingController != null || callingAction != null)
         {
            PushNavigationData( callingController, callingAction, callingId );
         }
         else
         {
            PopNavigationData();
         }
         PeekNavigationData( viewModel );
      }


      protected void ClearNavigationStack()
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack != null && stack.Count != 0)
         {
            stack.Clear();
            Session["NavigationStack"] = stack;
         }
      }


      private void PushNavigationData( string callingController, string callingAction, string callingId )
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack == null)
         {
            stack = new Stack<NavigationData>();
         }

         if (stack.Count != 0)
         {
            var top = stack.Peek();
            if (top.Controller == callingController && top.Action == callingAction && top.Id == callingId)
            {
               return;
            }
         }

         stack.Push( new NavigationData() { Controller = callingController, Action = callingAction, Id = callingId } );
         Session["NavigationStack"] = stack;
      }


      private void PopNavigationData()
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack != null && stack.Count != 0)
         {
            stack.Pop();
            Session["NavigationStack"] = stack;
         }
      }


      protected void PeekNavigationData( ViewModelBase viewModel )
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack != null && stack.Count != 0)
         {
            var data = stack.Peek();
            Guid parsedId;
            Guid.TryParse( data.Id, out parsedId );
            viewModel.CallingController = data.Controller;
            viewModel.CallingAction = data.Action;
            viewModel.CallingId = parsedId;
         }
         else
         {
            viewModel.CallingController = null;
            viewModel.CallingAction = null;
            viewModel.CallingId = Guid.Empty;
         }
      }


      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}