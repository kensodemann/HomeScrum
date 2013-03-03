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
   public class AcceptanceCriteriaStatusesController : Controller
   {
      #region Construction
      private readonly IDataObjectRepository<AcceptanceCriteriaStatus> _repository;

      public AcceptanceCriteriaStatusesController( IDataObjectRepository<AcceptanceCriteriaStatus> repository )
      {
         _repository = repository;
      }
      #endregion

      //
      // GET: /AcceptanceCriteriaStatuses/
      public ActionResult Index()
      {
         return View( _repository.GetAll() );
      }

      //
      // GET: /AcceptanceCriteriaStatuses/Details/5
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
      // GET: /AcceptanceCriteriaStatuses/Create
      public ActionResult Create()
      {
         return View();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Create
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
      // GET: /AcceptanceCriteriaStatuses/Edit/5
      public ActionResult Edit( int id )
      {
         return View();
      }

      //
      // POST: /AcceptanceCriteriaStatuses/Edit/5
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
   }
}
