using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class AdminController : Controller
   {
      #region Contstruction
      private readonly IDataObjectRepository<WorkItemStatus> _statusRepository;
      private readonly IDataObjectRepository<WorkItemType> _typeRepository;

      [Inject]
      public AdminController( IDataObjectRepository<WorkItemStatus> statusRepository,
                              IDataObjectRepository<WorkItemType> typeRepository )
      {
         _statusRepository = statusRepository;
         _typeRepository = typeRepository;
      }
      #endregion

      // TODOs:
      //
      // Immediate:
      //  Create tests for this.
      //  Create initial pages, etc for the other base items.
      //
      // Long Term:
      //  Create view models and use them instead.  This may move the repository injection points,
      //  especially if we inject the view models.

      // GET: /Admin/
      public ActionResult Index()
      {
         return View( );
      }


      // GET: /Admin/WorkItemTypes
      public ActionResult WorkItemTypes()
      {
         return View( _typeRepository.GetAll() );
      }


      // GET: /Admin/WorkItemStatuses
      public ActionResult WorkItemStatuses()
      {
         // Unltimately, we should have a view model for this instead of the data model
         return View( _statusRepository.GetAll() );
      }
   }
}
