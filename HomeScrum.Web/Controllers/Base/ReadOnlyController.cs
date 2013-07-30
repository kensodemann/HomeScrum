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
         ModelT model;
         Log.Debug( "Details(%s)", id.ToString() );

         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            model = session.Get<ModelT>( id );
            transaction.Commit();
         }

         if (model == null)
         {
            return HttpNotFound();
         }

         var viewModel = Mapper.Map<ViewModelT>( model );
         Guid parsedId;
         Guid.TryParse( callingId, out parsedId );
         viewModel.CallingAction = callingAction;
         viewModel.CallingId = parsedId;
         return View( viewModel );
      }

      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}