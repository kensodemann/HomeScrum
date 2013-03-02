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
      private readonly IDataObjectRepository<AcceptanceCriteriaStatus> _acceptenceCriteriaStatusRepository;
      private readonly IDataObjectRepository<ProjectStatus> _projectStatusRepository;
      private readonly IDataObjectRepository<SprintStatus> _sprintStatusRepository;
      private readonly IDataObjectRepository<WorkItemStatus> _workItemStatusRepository;
      private readonly IDataObjectRepository<WorkItemType> _workItemTypeRepository;

      [Inject]
      public AdminController( IDataObjectRepository<AcceptanceCriteriaStatus> acceptenceCriteriaStatusRepository,
                              IDataObjectRepository<ProjectStatus> projectStatusRepository,
                              IDataObjectRepository<SprintStatus> sprintStatusRepository,
                              IDataObjectRepository<WorkItemStatus> workItemStatusRepository,
                              IDataObjectRepository<WorkItemType> workItemTypeRepository )
      {
         _acceptenceCriteriaStatusRepository = acceptenceCriteriaStatusRepository;
         _projectStatusRepository = projectStatusRepository;
         _sprintStatusRepository = sprintStatusRepository;
         _workItemStatusRepository = workItemStatusRepository;
         _workItemTypeRepository = workItemTypeRepository;
      }
      #endregion

      // TODOs:
      //
      // Immediate:
      //  Create initial pages, etc for the other base items.
      //
      // Long Term:
      //  Create view models and use them instead.  This may move the repository injection points,
      //  especially if we inject the view models.

      // GET: /Admin/
      public ActionResult Index()
      {
         return View();
      }

      // GET: /Admin/SprintStatuses
      public ActionResult SprintStatuses()
      {
         return View( _sprintStatusRepository.GetAll() );
      }

      // GET: /Admin/WorkItemStatuses
      public ActionResult WorkItemStatuses()
      {
         // Unltimately, we should have a view model for this instead of the data model
         return View( _workItemStatusRepository.GetAll() );
      }

      // GET: /Admin/WorkItemTypes
      public ActionResult WorkItemTypes()
      {
         return View( _workItemTypeRepository.GetAll() );
      }
   }
}
