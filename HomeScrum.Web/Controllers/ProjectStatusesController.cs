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
   public class ProjectStatusesController : Controller
   {
      #region Construction
      private readonly IDataObjectRepository<ProjectStatus> _repository;

      [Inject]
      public ProjectStatusesController( IDataObjectRepository<ProjectStatus> repository )
      {
         _repository = repository;
      }
      #endregion


      //
      // GET: /ProjectStatuses/

      public ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /ProjectStatuses/Details/5

      public ActionResult Details( int id )
      {
         return View();
      }

      //
      // GET: /ProjectStatuses/Create

      public ActionResult Create()
      {
         return View();
      }

      //
      // POST: /ProjectStatuses/Create

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
      // GET: /ProjectStatuses/Edit/5

      public ActionResult Edit( int id )
      {
         return View();
      }

      //
      // POST: /ProjectStatuses/Edit/5

      [HttpPost]
      public ActionResult Edit( int id, FormCollection collection )
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

      //
      // GET: /ProjectStatuses/Delete/5

      public ActionResult Delete( int id )
      {
         return View();
      }

      //
      // POST: /ProjectStatuses/Delete/5

      [HttpPost]
      public ActionResult Delete( int id, FormCollection collection )
      {
         try
         {
            // TODO: Add delete logic here

            return RedirectToAction( "Index" );
         }
         catch
         {
            return View();
         }
      }
   }
}
