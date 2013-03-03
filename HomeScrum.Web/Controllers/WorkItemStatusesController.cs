using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemStatusesController : Controller
   {
      #region Construction
      private readonly IDataObjectRepository<WorkItemStatus> _repository;

      [Inject]
      public WorkItemStatusesController( IDataObjectRepository<WorkItemStatus> repository )
      {
         _repository = repository;
      }
      #endregion


      //
      // GET: /WorkItemStatuses/
      public ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /WorkItemStatuses/Details/5
      public ActionResult Details( Guid id )
      {
         var model = _repository.Get( id );

         if (model == null)
         {
            return HttpNotFound();
         }

         return View( model );
      }

      //
      // GET: /WorkItemStatuses/Create
      public ActionResult Create()
      {
         return View();
      }

      //
      // POST: /WorkItemStatuses/Create
      [HttpPost]
      public ActionResult Create( FormCollection collection )
      {
         try
         {
            // TODO: Add insert logic here

            return RedirectToAction( "Index" );
         }
         catch
         {
            return View();
         }
      }

      //
      // GET: /WorkItemStatuses/Edit/5
      public ActionResult Edit( Guid id )
      {
         return View();
      }

      //
      // POST: /WorkItemStatuses/Edit/5
      [HttpPost]
      public ActionResult Edit( Guid id, FormCollection collection )
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
