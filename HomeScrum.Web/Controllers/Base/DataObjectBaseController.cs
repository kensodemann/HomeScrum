using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public class DataObjectBaseController<T> : Controller where T : DataObjectBase
   {
      public DataObjectBaseController( IDataObjectRepository<T> repository )
      {
         _repository = repository;
      }

      private readonly IDataObjectRepository<T> _repository;
      public IDataObjectRepository<T> Repository { get { return _repository; } }

      //
      // GET: /AcceptanceCriteriaStatuses/
      public virtual ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Details/5
      public virtual ActionResult Details( Guid id )
      {
         var model = _repository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }
         return View( model );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Create
      public virtual ActionResult Create()
      {
         return View();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Create
      [HttpPost]
      public virtual ActionResult Create( T model )
      {
         if (ModelState.IsValid)
         {
            _repository.Add( model );
            return RedirectToAction( "Index" );
         }

         return View();
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Edit/5
      public virtual ActionResult Edit( Guid id )
      {
         var model = _repository.Get( id );
         return View();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Edit/5
      [HttpPost]
      public virtual ActionResult Edit( Guid id, FormCollection collection )
      {
         try
         {
            // TODO: Add update logic here

            return RedirectToAction( "Index" );
         }
         catch
         {
            return View();
         }
      }
   }
}