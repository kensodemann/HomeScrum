using AutoMapper;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Base;
using NHibernate;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   /// <summary>
   /// The ReadOnlyController is the base class for any contoller in the system that only supports
   /// the GET operations.  The only actions for this type of controller are Index and Details.
   /// </summary>
   /// <typeparam name="ModelT">The Domain Model Type for the main data</typeparam>
   /// <typeparam name="ViewModelT">The View Model Type for display views</typeparam>
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
         IEnumerable<DomainObjectViewModel> items;
         Log.Debug( "Index()" );

         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var queryModel = new HomeScrum.Data.Queries.AllDomainObjects<ModelT>();
            items = queryModel.GetQuery( session )
               .SelectDomainObjectViewModels<ModelT>();

            transaction.Commit();
         }

         return View( items );
      }

      //
      // GET: /ModelTs/Details/Guid
      public virtual ActionResult Details( Guid id, string callingAction = null, string callingId = null )
      {
         ViewModelT viewModel;
         Log.Debug( "Details(%s)", id.ToString() );

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

         UpdateNavigationStack( viewModel, callingAction, callingId );

         return View( viewModel );
      }


      protected virtual ViewModelT GetViewModel( ISession session, Guid id )
      {
         var model = session.Get<ModelT>( id );
         return (model != null) ? Mapper.Map<ViewModelT>( model ) : null;
      }


      protected void UpdateNavigationStack( ViewModelBase viewModel, string callingAction, string callingId )
      {
         if (callingAction != null)
         {
            PushNavigationData( callingAction, callingId );
         }
         else
         {
            PopNavigationData();
         }
         PeekNavigationData( viewModel );
      }


      private void PushNavigationData( string callingAction, string callingId )
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack == null)
         {
            stack = new Stack<NavigationData>();
         }

         if (stack.Count != 0)
         {
            var top = stack.Peek();
            if (top.Action == callingAction && top.Id == callingId)
            {
               return;
            }
         }
         
         stack.Push( new NavigationData() { Action = callingAction, Id = callingId } );
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


      private void PeekNavigationData( ViewModelBase viewModel )
      {
         var stack = Session["NavigationStack"] as Stack<NavigationData>;
         if (stack != null && stack.Count != 0)
         {
            var data = stack.Peek();
            Guid parsedId;
            Guid.TryParse( data.Id, out parsedId );
            viewModel.CallingAction = data.Action;
            viewModel.CallingId = parsedId;
         }
         else
         {
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