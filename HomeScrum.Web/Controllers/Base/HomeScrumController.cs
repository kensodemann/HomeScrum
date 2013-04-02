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
      private readonly IRepository<ModelT, Guid> _repository;
      protected IRepository<ModelT, Guid> Repository { get { return _repository; } }

      public HomeScrumController( IRepository<ModelT, Guid> repository )
      {
         _repository = repository;
      }


      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}