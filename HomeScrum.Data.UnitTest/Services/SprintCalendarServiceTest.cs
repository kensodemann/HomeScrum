using System;
using HomeScrum.Common.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Services;
using Ninject.Extensions.Logging;
using System.Collections.Generic;

namespace HomeScrum.Data.UnitTest.Services
{
   [TestClass]
   public class SprintCalendarServiceTest
   {
      #region Test Initialization
      private Mock<ILogger> _logger;
      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private ISprintCalendarService _service;

      [ClassInitialize]
      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         SetupMocks();
         SetupSession();

         SetupDatabase();

         _service = new SprintCalendarService( _logger.Object, _sessionFactory.Object );
      }

      private void SetupDatabase()
      {
         Database.Build( _session );
         Sprints.Load( _sessionFactory.Object );
         WorkItems.Load( _sessionFactory.Object );
         WorkItemDailySnapshots.Load( _sessionFactory.Object );

      }

      private void SetupMocks()
      {
         _logger = new Mock<ILogger>();
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion


      [TestMethod]
      public void Update_DoesNothingIfNoStartDate()
      {
         var sprint = _session.Query<WorkItem>().First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 ).Sprint;
         var calCount = sprint.Calendar.Count();
         var calHash = sprint.Calendar.GetHashCode();

         sprint.StartDate = null;
         _service.Update( sprint );

         Assert.AreEqual( calCount, sprint.Calendar.Count() );
         Assert.AreEqual( calHash, sprint.Calendar.GetHashCode() );
      }

      [TestMethod]
      public void Update_DoesNothingIfNoEndDate()
      {
         var sprint = _session.Query<WorkItem>().First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 ).Sprint;
         var calCount = sprint.Calendar.Count();
         var calHash = sprint.Calendar.GetHashCode();

         sprint.EndDate = null;
         _service.Update( sprint );

         Assert.AreEqual( calCount, sprint.Calendar.Count() );
         Assert.AreEqual( calHash, sprint.Calendar.GetHashCode() );
      }

      [TestMethod]
      public void Update_AddsDaysUpToToday_IfNotThere()
      {
         var sprint = _session.Query<WorkItem>()
            .First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 && x.Sprint.StartDate < DateTime.Now.Date && x.Sprint.EndDate > DateTime.Now.Date )
            .Sprint;
         var sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();

         sprint.Calendar.Clear();
         _service.Update( sprint );

         Assert.AreEqual( 16, sprint.Calendar.Count() );
         for (int i = 0; i < 16; i++)
         {
            AssertSprintCalendar( sprint, sprintTasks, i );
         }
      }

      [TestMethod]
      public void Update_UpdatesTodayIfEntryExists()
      {
         var sprint = _session.Query<WorkItem>()
            .First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 && x.Sprint.StartDate < DateTime.Now.Date && x.Sprint.EndDate > DateTime.Now.Date )
            .Sprint;
         var sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();

         sprint.Calendar.Clear();
         _service.Update( sprint );
         sprint.Calendar.Single( x => x.HistoryDate == DateTime.Now.Date ).PointsRemaining += 143;
         _service.Update( sprint );

         Assert.AreEqual( 16, sprint.Calendar.Count() );
         for (int i = 0; i < 16; i++)
         {
            AssertSprintCalendar( sprint, sprintTasks, i );
         }
      }

      [TestMethod]
      public void Update_DoesNothingIfTodayAfterEndDate()
      {
      }

      [TestMethod]
      public void Update_DoesNothingIfTodayBeforeStartDate()
      {

      }

      [TestMethod]
      public void Reset_DoesNothingIfNoStartDate()
      {

      }

      [TestMethod]
      public void Reset_DoesNothingIfNoEndDate()
      {

      }

      [TestMethod]
      public void Reset_CreatesEntryForStartDate_IfCurrentDateBeforeStartDate()
      {

      }

      [TestMethod]
      public void Reset_RebuildsCalendarToDate_IfCurrentDateBetweenStartAndEndDate()
      {

      }

      [TestMethod]
      public void Reset_RebuildsFullCalendar_IfCurrentDatePastEndDate()
      {

      }

      private void AssertSprintCalendar( Sprint sprint, IEnumerable<WorkItem> tasks, int day )
      {
         var date = ((DateTime)sprint.StartDate).AddDays( day );
         var pointsRemaining = 0;
         foreach (var task in tasks)
         {
            pointsRemaining += task.PointsHistory.OrderBy( x => x.HistoryDate ).Last( x => x.HistoryDate <= date ).PointsRemaining;
         }
         Assert.AreEqual( pointsRemaining, sprint.Calendar.Single( x => x.HistoryDate == date ).PointsRemaining );
      }
   }
}
