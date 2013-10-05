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
   /// the GET operations.  The only action for this type of controller is Index.
   /// </summary>
   /// <typeparam name="ModelT">The Domain Model Type for the main data</typeparam>
   [Authorize]
   public abstract class ReadOnlyController<ModelT> : Controller
      where ModelT : DomainObjectBase
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


      private void PeekNavigationData( ViewModelBase viewModel )
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