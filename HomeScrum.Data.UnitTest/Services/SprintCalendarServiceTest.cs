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
         var calHash = CalendarHash( sprint );

         sprint.StartDate = null;
         _service.Update( sprint );

         Assert.AreEqual( calCount, sprint.Calendar.Count() );
         Assert.AreEqual( calHash, CalendarHash( sprint ) );
      }

      [TestMethod]
      public void Update_DoesNothingIfNoEndDate()
      {
         var sprint = _session.Query<WorkItem>().First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 ).Sprint;
         var calCount = sprint.Calendar.Count();
         var calHash = CalendarHash( sprint );

         sprint.EndDate = null;
         _service.Update( sprint );

         Assert.AreEqual( calCount, sprint.Calendar.Count() );
         Assert.AreEqual( calHash, CalendarHash( sprint ) );
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

         Assert.AreEqual( 16, sprint.Calendar.Count );
         for (int i = 0; i < sprint.Calendar.Count; i++)
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
         var sprint = _session.Query<WorkItem>()
            .First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 && x.Sprint.StartDate != null && x.Sprint.EndDate < DateTime.Now.Date )
            .Sprint;
         var calCount = sprint.Calendar.Count();
         var calHash = CalendarHash( sprint );

         _service.Update( sprint );

         Assert.AreEqual( calCount, sprint.Calendar.Count() );
         Assert.AreEqual( calHash, CalendarHash( sprint ) );
      }

      [TestMethod]
      public void Update_UpdatesFirstDayOfSprint_IfTodayBeforeStartDate()
      {
         var sprint = _session.Query<WorkItem>()
            .First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 && x.Sprint.StartDate < DateTime.Now.Date && x.Sprint.EndDate > DateTime.Now.Date )
            .Sprint;
         var sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();

         sprint.StartDate = DateTime.Now.AddDays( 1 ).Date;
         sprint.EndDate = DateTime.Now.AddDays( 31 ).Date;
         sprint.Calendar.Clear();
         _service.Update( sprint );

         Assert.AreEqual( 1, sprint.Calendar.Count() );
         AssertSprintCalendar( sprint, sprintTasks, 0 );
      }

      [TestMethod]
      public void Reset_RemovesCalendarIfNoStartDate()
      {
         var sprint = _session.Query<WorkItem>().First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 ).Sprint;

         sprint.StartDate = null;
         _service.Reset( sprint );

         Assert.AreEqual( 0, sprint.Calendar.Count() );
      }

      [TestMethod]
      public void Reset_RemovesCalendarIfNoEndDate()
      {
         var sprint = _session.Query<WorkItem>().First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 ).Sprint;

         sprint.EndDate = null;
         _service.Reset( sprint );

         Assert.AreEqual( 0, sprint.Calendar.Count() );
      }

      [TestMethod]
      public void Reset_CreatesEntryForStartDate_IfCurrentDateBeforeStartDate()
      {
         var sprint = _session.Query<WorkItem>()
            .First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 && x.Sprint.StartDate < DateTime.Now.Date && x.Sprint.EndDate > DateTime.Now.Date )
            .Sprint;
         var sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();

         sprint.StartDate = DateTime.Now.AddDays( 1 ).Date;
         sprint.EndDate = DateTime.Now.AddDays( 31 ).Date;
         _service.Reset( sprint );

         Assert.AreEqual( 1, sprint.Calendar.Count() );
         AssertSprintCalendar( sprint, sprintTasks, 0 );
      }

      [TestMethod]
      public void Reset_RebuildsCalendarToDate_IfCurrentDateBetweenStartAndEndDate()
      {
         var sprint = _session.Query<WorkItem>()
            .First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 && x.Sprint.StartDate < DateTime.Now.Date && x.Sprint.EndDate > DateTime.Now.Date )
            .Sprint;
         var sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();

         sprint.StartDate = DateTime.Now.AddDays( -10 ).Date;
         sprint.EndDate = DateTime.Now.AddDays( 21 ).Date;
         _service.Reset( sprint );

         Assert.AreEqual( 11, sprint.Calendar.Count() );
         for (int i = 0; i < sprint.Calendar.Count; i++)
         {
            AssertSprintCalendar( sprint, sprintTasks, i );
         }
      }

      [TestMethod]
      public void Reset_RebuildsFullCalendar_IfCurrentDatePastEndDate()
      {
         var sprint = _session.Query<WorkItem>()
         .First( x => x.Sprint != null && x.Sprint.Calendar.Count() > 0 && x.Sprint.StartDate < DateTime.Now.Date && x.Sprint.EndDate > DateTime.Now.Date )
         .Sprint;
         var sprintTasks = _session.Query<WorkItem>()
            .Where( x => x.Sprint.Id == sprint.Id && x.WorkItemType.Category != WorkItemTypeCategory.BacklogItem )
            .ToList();

         sprint.StartDate = DateTime.Now.AddDays( -10 ).Date;
         sprint.EndDate = DateTime.Now.AddDays( -1 ).Date;
         _service.Reset( sprint );

         Assert.AreEqual( 10, sprint.Calendar.Count() );
         for (int i = 0; i < sprint.Calendar.Count; i++)
         {
            AssertSprintCalendar( sprint, sprintTasks, i );
         }
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

      private int CalendarHash( Sprint sprint )
      {
         var hash = 0;
         foreach (var e in sprint.Calendar)
         {
            hash += e.GetHashCode();
         }
         return hash;
      }
   }
}
