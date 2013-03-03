using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
    public class WorkItemTypesController : Controller
    {
        //
        // GET: /WorkItemTypes/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /WorkItemTypes/Details/5

        public ActionResult Details(int id)
        {
            return View();
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
        // GET: /WorkItemTypes/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /WorkItemTypes/Edit/5

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
        // GET: /WorkItemTypes/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /WorkItemTypes/Delete/5

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
