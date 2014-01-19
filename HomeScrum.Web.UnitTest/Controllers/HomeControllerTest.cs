using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Home;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.MockingKernel.Moq;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class HomeControllerTest
   {
      #region Test Setup
      private HomeController _controller;

      private static MoqMockingKernel _iocKernel;

      private Mock<ILogger> _logger;

      private User _user;
      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();

         IntializeMapper();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         SetupSession();
         CreateMockIOCKernel();
         BuildDatabase();

         SetupCurrentUser();
         SetupLogger();

         CreateController();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }


      private void BuildDatabase()
      {
         Database.Build( _session );
         Sprints.Load( _sessionFactory.Object );
         WorkItemHistories.Load( _sessionFactory.Object );
      }

      private void CreateController()
      {
         _controller = new HomeController( _logger.Object, _sessionFactory.Object );
      }

      private void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( _sessionFactory.Object );
      }

      private static void IntializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      private void SetupCurrentUser()
      {
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         var userInfo = WorkItemHistories.ModelData
            .GroupBy( x => x.LastModifiedUser.Id )
            .Select( g => new { Id = g.Key, EntryCount = g.Count() } )
            .OrderBy( x => x.EntryCount )
            .Last();

         _user = Users.ModelData.Single( x => x.Id == userInfo.Id );
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( _user.UserName );
      }

      private void SetupLogger()
      {
         _logger = new Mock<ILogger>();
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }
      #endregion


      [TestMethod]
      public void About_ReturnsViewWithoutModel()
      {
         var view = _controller.About() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNull( view.Model );
      }


      [TestMethod]
      public void Index_ReturnsViewWithSnapshotViewModel()
      {
         var view = _controller.Index( _principal.Object ) as ViewResult;

         Assert.IsNotNull( view, "view" );
         Assert.IsNotNull( view.Model, "view model" );
      }


      [TestMethod]
      public void Index_PopulatesLatestSprintsList()
      {
         var view = _controller.Index( _principal.Object ) as ViewResult;
         var viewModel = view.Model as Snapshot;

         Assert.IsTrue( viewModel.LatestSprints.Count() > 0, "At Least One" );
         Assert.IsTrue( viewModel.LatestSprints.Count() <= 5, "No More than Five" );
      }


      [TestMethod]
      public void AllSprintsInLatestSprintListAreOpen()
      {
         var view = _controller.Index( _principal.Object ) as ViewResult;
         var viewModel = view.Model as Snapshot;

         foreach (var item in viewModel.LatestSprints)
         {
            var sprint = Sprints.ModelData.Single( x => x.Id == item.Id );
            Assert.IsTrue( sprint.Status.Category != SprintStatusCategory.Complete );
         }
      }


      [TestMethod]
      public void AllSprintsInLatestSprintListStartedOrStartWithinThirtyDays()
      {
         var view = _controller.Index( _principal.Object ) as ViewResult;
         var viewModel = view.Model as Snapshot;

         foreach (var item in viewModel.LatestSprints)
         {
            var sprint = Sprints.ModelData.Single( x => x.Id == item.Id );
            Assert.IsTrue( sprint.StartDate != null && sprint.StartDate <= DateTime.Now.Date.AddDays( 30 ) );
         }
      }


      [TestMethod]
      public void Index_GetsTheLatestFiveWorkItemsEntered()
      {
         var view = _controller.Index( _principal.Object ) as ViewResult;
         var viewModel = view.Model as Snapshot;

         var expected = WorkItemHistories.ModelData
            .Where( x => x.SequenceNumber == 1 )
            .OrderByDescending( x => x.HistoryTimestamp )
            .ToArray();
         var actual = viewModel.NewestWorkItems.ToArray();

         Assert.AreEqual( 5, viewModel.NewestWorkItems.Count() );

         for (var i = 0; i < 5; i++)
         {
            Assert.AreEqual( expected[i].WorkItem.Id, actual[i].Id );
            Assert.AreEqual( expected[i].WorkItem.Name, actual[i].Name );
         }
      }


      [TestMethod]
      public void Index_GetsTheLatestFiveWorkItemsModifiedByTheCurrentUser()
      {
         var view = _controller.Index( _principal.Object ) as ViewResult;
         var viewModel = view.Model as Snapshot;

         var expected = WorkItemHistories.ModelData
            .Where( x => x.LastModifiedUser.Id == _user.Id )
            .GroupBy( x => x.WorkItem.Id )
            .Select( g => new { Id = g.Key, HistoryDate = g.Max( x => x.HistoryTimestamp ) } )
            .OrderByDescending( x => x.HistoryDate )
            .ToArray();
         var actual = viewModel.RecentActivity.ToArray();

         Assert.AreEqual( 5, viewModel.NewestWorkItems.Count() );

         for (var i = 0; i < 5; i++)
         {
            var workItem = WorkItems.ModelData.Single( x => x.Id == expected[i].Id );
            Assert.AreEqual( workItem.Id, actual[i].Id );
            Assert.AreEqual( workItem.Name, actual[i].Name );
         }
      }


      [TestMethod]
      public void Index_GetsSprintCalendarsForCurrentSprints()
      {
         var view = _controller.Index( _principal.Object ) as ViewResult;
         var viewModel = view.Model as Snapshot;

         var expectedSprints = Sprints.ModelData
            .Where( x => x.StartDate != null && x.StartDate <= DateTime.Now.Date && x.EndDate != null && x.EndDate >= DateTime.Now.Date
               && x.Status.Category == SprintStatusCategory.Active ).ToList();
         var actual = viewModel.BurndownCharts;

         Assert.AreEqual( expectedSprints.Count, actual.Count() );
         foreach(var sprint in expectedSprints)
         {
            var current = actual.SingleOrDefault( x => x.Id == sprint.Id );
            Assert.IsNotNull( current );
            Assert.AreEqual( sprint.Calendar.Count, current.Calendar.Count() );

            foreach(var entry in sprint.Calendar)
            {
               Assert.IsNotNull( current.Calendar.SingleOrDefault( x => x.HistoryDate == entry.HistoryDate ) );
            }
         }
      }
   }
}
