using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
    public class SprintStatusesController : Controller
    {
        //
        // GET: /SprintStatuses/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /SprintStatuses/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /SprintStatuses/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SprintStatuses/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /SprintStatuses/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SprintStatuses/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /SprintStatuses/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /SprintStatuses/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
