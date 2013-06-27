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

      private readonly ILogger _logger;
      protected ILogger Log { get { return _logger; } }

      public ReadOnlyController( IRepository<ModelT> mainRepository, ILogger logger )
      {
         _repository = mainRepository;
         _logger = logger;
      }

      //
      // GET: /ModelTs/
      public virtual ActionResult Index()
      {
         var items = MainRepository.GetAll();
         return View( Mapper.Map<ICollection<ModelT>, IEnumerable<ViewModelT>>( items ) );
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