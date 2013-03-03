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
   public class WorkItemTypesController : Controller
   {
      #region Construction
      private readonly IDataObjectRepository<WorkItemType> _repository;

      [Inject]
      public WorkItemTypesController( IDataObjectRepository<WorkItemType> repository )
      {
         _repository = repository;
      }
      #endregion


      //
      // GET: /WorkItemTypes/
      public ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /WorkItemTypes/Details/5
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
      // GET: /WorkItemTypes/Create
      public ActionResult Create()
      {
         return View();
      }

      //
      // POST: /WorkItemTypes/Create
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
      // GET: /WorkItemTypes/Edit/5
      public ActionResult Edit( Guid id )
      {
         return View();
      }

      //
      // POST: /WorkItemTypes/Edit/5
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
