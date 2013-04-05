using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Repositories;

namespace HomeScrum.Web.Controllers.Base
{
   public class HomeScrumController<ModelT> : Controller
   {
      private readonly IRepository<ModelT> _repository;
      protected IRepository<ModelT> Repository { get { return _repository; } }

      public HomeScrumController( IRepository<ModelT> repository )
      {
         _repository = repository;
      }

      //
      // GET: /ModelTs/
      public virtual ActionResult Index()
      {
         var items = Repository.GetAll();
         return View( items );
      }

      //
      // GET: /ModelTs/Details/Guid
      public virtual ActionResult Details( Guid id )
      {
         var model = Repository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( model );
      }

      //
      // GET: /ModelTs/Create
      public virtual ActionResult Create()
      {
         return View();
      }

      //
      // GET: /ModelTs/Edit/Guid
      public virtual ActionResult Edit( Guid id )
      {
         var model = Repository.Get( id );

         if (model != null)
         {
            return View( model );
         }

         return HttpNotFound();
      }

      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}