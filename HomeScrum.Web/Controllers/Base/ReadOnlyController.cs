using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Repositories;
using AutoMapper;
using HomeScrum.Web.Models.Base;
using Ninject.Extensions.Logging;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

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
   {
      private readonly IRepository<ModelT> _repository;
      protected IRepository<ModelT> MainRepository { get { return _repository; } }

      private readonly ISessionFactory _sessionFactory;
      protected ISessionFactory SessionFactory { get { return _sessionFactory; } }

      private readonly ILogger _logger;
      protected ILogger Log { get { return _logger; } }

      public ReadOnlyController( IRepository<ModelT> mainRepository, ILogger logger, ISessionFactory sessionFactory )
      {
         _repository = mainRepository;
         _logger = logger;
         _sessionFactory = sessionFactory;
      }

      //
      // GET: /ModelTs/
      public virtual ActionResult Index()
      {
         Log.Debug( "Index()" );

         using (var session = SessionFactory.OpenSession())
         {
            var items = session
               .CreateCriteria( typeof( ModelT ) )
               .SetProjection( Projections.ProjectionList()
                  .Add( Projections.Property( "Id" ), "Id" )
                  .Add( Projections.Property( "Name" ), "Name" )
                  .Add( Projections.Property( "Description" ), "Description" )
                  .Add( Projections.Property( "StatusCd" ), "StatusCd" )
                  .Add( Projections.Property( "IsPredefined" ), "IsPredefined" ) )
               .SetResultTransformer( Transformers.AliasToBean<SystemDomainObjectViewModel>() )
               .List<SystemDomainObjectViewModel>();
            return View( items );
         }
      }

      //
      // GET: /ModelTs/Details/Guid
      public virtual ActionResult Details( Guid id )
      {
         var model = MainRepository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( Mapper.Map<ViewModelT>( model ) );
      }

      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}