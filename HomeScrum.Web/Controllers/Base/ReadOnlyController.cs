﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Repositories;
using AutoMapper;
using HomeScrum.Web.Models.Base;

namespace HomeScrum.Web.Controllers.Base
{
   /// <summary>
   /// The ReadOnlyController is the base class for any contoller in the system that only supports
   /// the GET operations.  The only actions for this type of controller are Index and Details.
   /// </summary>
   /// <typeparam name="ModelT">The Domain Model Type for the main data</typeparam>
   [Authorize]
   public abstract class ReadOnlyController<ModelT> : Controller
   {
      private readonly IRepository<ModelT> _repository;
      protected IRepository<ModelT> MainRepository { get { return _repository; } }

      public ReadOnlyController( IRepository<ModelT> mainRepository )
      {
         _repository = mainRepository;
      }

      //
      // GET: /ModelTs/
      public abstract ActionResult Index();

      //
      // GET: /ModelTs/Details/Guid
      public virtual ActionResult Details( Guid id )
      {
         var model = MainRepository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( Mapper.Map<DisplayViewModel>( model ) );
      }

      protected internal RedirectToRouteResult RedirectToAction<T>( Expression<Func<T>> expression )
      {
         var actionName = ClassHelper.ExtractMethodName( expression );
         return this.RedirectToAction( actionName );
      }
   }
}