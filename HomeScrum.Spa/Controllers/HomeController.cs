using System.Web.Mvc;

namespace HomeScrum.Spa.Controllers
{
   public class HomeController : Controller
   {
      public ActionResult Index()
      {
         return View();
      }
   }
}