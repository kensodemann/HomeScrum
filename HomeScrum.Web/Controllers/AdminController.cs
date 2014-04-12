using System;
using System.Web.Mvc;
using HomeScrum.Web.Attributes;

namespace HomeScrum.Web.Controllers
{
   [Authorize]
   public class AdminController : Controller
   {
      // GET: /Admin/
      [ReleaseRequireHttps]
      public ActionResult Index()
      {
         return View();
      }
   }
}
