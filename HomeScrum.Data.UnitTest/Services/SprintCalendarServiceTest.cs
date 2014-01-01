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
         WorkItemDailySnapshots.Load( _sessionFactory.Object );
         var sprint = _session.Query<WorkItem>().First( x => x.Sprint != null && x.PointsHistory.Count() > 0 ).Sprint;
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

      }

      [TestMethod]
      public void Update_AddsDaysUpToToday_IfNotThere()
      {
      }

      [TestMethod]
      public void Update_UpdatesTodayIfEntryExists()
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
   }
}
