using System.Collections.Generic;
using System.Web.Mvc;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.Home;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;
using HomeScrum.Data.Queries;
using NHibernate.Linq;
using System.Linq;
using System;
using HomeScrum.Web.Extensions;
using System.Security.Principal;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Attributes;

namespace HomeScrum.Web.Controllers
{
   [Authorize]
   public class HomeController : Controller
   {
      private readonly ISessionFactory _sessionFactory;
      protected ISessionFactory SessionFactory { get { return _sessionFactory; } }

      private readonly ILogger _logger;
      protected ILogger Log { get { return _logger; } }


      [Inject]
      public HomeController( ILogger logger, ISessionFactory sessionFactory )
      {
         _logger = logger;
         _sessionFactory = sessionFactory;
      }

      [ReleaseRequireHttps]
      public ActionResult Index( IPrincipal user )
      {
         var viewModel = new Snapshot()
         {
            LatestSprints = GetLatestSprints(),
            NewestWorkItems = GetNewestWorkItems(),
            RecentActivity = GetRecentActivity( user ),
            BurndownCharts = GetBurndownCharts()
         };

         return View( viewModel );
      }


      private IEnumerable<DomainObjectViewModel> GetLatestSprints()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session
            .OpenSprints()
            .Where( x => x.StartDate != null && x.StartDate <= DateTime.Now.Date.AddDays( 30 ) )
            .OrderByDescending( x => x.StartDate )
            .Take( 5 )
            .Select( x => new DomainObjectViewModel()
            {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description
            } )
            .ToList();
      }


      private IEnumerable<DomainObjectViewModel> GetNewestWorkItems()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session.Query<WorkItemHistory>()
            .Where( x => x.SequenceNumber == 1 )
            .OrderByDescending( x => x.HistoryTimestamp )
            .Take( 5 )
            .Select( x => new DomainObjectViewModel()
            {
               Id = x.WorkItem.Id,
               Name = x.WorkItem.Name + " (" + x.WorkItem.Project.Name + ")",
               Description = x.WorkItem.Description
            } )
            .ToList();
      }


      private IEnumerable<DomainObjectViewModel> GetRecentActivity( IPrincipal user )
      {
         var session = _sessionFactory.GetCurrentSession();
         var userId = user.Identity.GetUserId( session );

         var data = session.Query<WorkItemHistory>()
            .Where( x => x.LastModifiedUser.Id == userId )
            .GroupBy( x => new
            { 
               Id = x.WorkItem.Id,
               Name = x.WorkItem.Name,
               ProjectName = x.WorkItem.Project.Name,
               Description = x.WorkItem.Description
            } )
            .Select( g => new
            {
               Id = g.Key.Id,
               Name = g.Key.Name + " (" + g.Key.ProjectName + ")",
               Description = g.Key.Description,
               HistoryDate = g.Max( x => x.HistoryTimestamp )
            } ).ToList();

         return data
            .OrderByDescending( x => x.HistoryDate )
            .Take( 5 )
            .Select( x => new DomainObjectViewModel()
            {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description
            } );
      }


      private IEnumerable<IBurndownChart> GetBurndownCharts()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session
            .CurrentSprints()
            .Where( x => x.EndDate != null )
            .Select( x => new SprintBurndown()
            {
               Id = x.Id,
               Name = x.Name,
               StartDate = x.StartDate,
               EndDate = x.EndDate,
               Calendar = x.Calendar.Select( c => new SprintCalendarEntryViewModel()
               {
                  HistoryDate = c.HistoryDate,
                  PointsRemaining = c.PointsRemaining
               } )
            } );
      }


      [AllowAnonymous]
      public ActionResult About()
      {
         return View();
      }
   }
}
