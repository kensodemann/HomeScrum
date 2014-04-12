using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Attributes;
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
   public abstract class ReadOnlyController<ModelT, ViewModelT> : HomeScrumControllerBase
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
      [ReleaseRequireHttps]
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
      [ReleaseRequireHttps]
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
   }
}