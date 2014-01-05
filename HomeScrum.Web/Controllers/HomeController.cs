using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   [Authorize]
   public class HomeController : Controller
   {
      public ActionResult Index()
      {
         return View();
      }

      [AllowAnonymous]
      public ActionResult About()
      {
         return View();
      }
   }
}
