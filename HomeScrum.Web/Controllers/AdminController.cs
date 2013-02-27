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
      // GET: /Admin/
      public ActionResult Index()
      {
         return View( );
      }


      // GET: /Admin/WorkItemTypes
      public ActionResult WorkItemTypes()
      {
         // TODO: Inject this...
         IDataObjectRepository<WorkItemType> repository = new DataObjectRepository<WorkItemType>();

         return View( repository.GetAll() );
      }


      // GET: /Admin/WorkItemStatuses
      public ActionResult WorkItemStatuses()
      {
         // TODO: Inject this...
         // Unltimately, we should have a view model for this instead of the data model
         IDataObjectRepository<WorkItemStatus> repository = new DataObjectRepository<WorkItemStatus>();

         return View( repository.GetAll() );
      }
   }
}
