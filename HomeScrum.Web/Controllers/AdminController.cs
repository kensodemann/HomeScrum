using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Domain;
using HomeScrum.Data.SqlServer;

namespace HomeScrum.Web.Controllers
{
   public class AdminController : Controller
   {
      public ActionResult Index()
      {
         // TODO: Inject this...
         IDataObjectRepository<WorkItemType> repository = new DataObjectRepository<WorkItemType>();

         return View( repository.GetAll() );
      }

   }
}
