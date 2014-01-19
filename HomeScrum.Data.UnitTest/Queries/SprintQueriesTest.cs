using System;
using System.Linq;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;

namespace HomeScrum.Data.UnitTest.Queries
{
   [TestClass]
   public class SprintQueriesTest
   {
      #region Test Initialization
      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      [ClassInitialize]
      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );

         Database.Build( _session );
         Sprints.Load( _sessionFactory.Object );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion


      [TestMethod]
      public void AllSprints_ReturnsAllSprints()
      {
         var ret = _session.AllSprints().ToList();
         Assert.AreEqual( Sprints.ModelData.Count(), ret.Count );
         foreach (var s in Sprints.ModelData)
         {
            Assert.IsNotNull( ret.SingleOrDefault( x => x.Id == s.Id ) );
         }
      }


      [TestMethod]
      public void AllSprints_OrdersSprintsProperly()
      {
         var prevProject = "";
         var prevSortSeqNum = 0;
         var prevStartDate = DateTime.MinValue;

         var ret = _session.AllSprints().ToList();
         foreach (var s in ret)
         {
            Assert.IsTrue( String.Compare( prevProject, s.Project.Name ) <= 0, "Projects out of order" );
            if (prevProject == s.Project.Name)
            {
               Assert.IsTrue( prevStartDate <= (s.StartDate ?? DateTime.MaxValue), "date out of order" );
               if (prevStartDate == s.StartDate)
               {
                  Assert.IsTrue( prevSortSeqNum < s.Status.SortSequence, "sequence out of order" );
               }
            }

            prevProject = s.Project.Name;
            prevStartDate = s.StartDate ?? DateTime.MaxValue;
            prevSortSeqNum = s.Status.SortSequence;
         }
      }


      [TestMethod]
      public void OpenSprints_ReturnsSprintsThatAreNotComplete()
      {
         var expectedSprints = Sprints.ModelData.Where( x => x.Status.StatusCd == 'A' && x.Status.Category != Domain.SprintStatusCategory.Complete );

         var ret = _session.OpenSprints().ToList();

         Assert.AreEqual( expectedSprints.Count(), ret.Count );
         foreach (var s in expectedSprints)
         {
            Assert.IsNotNull( ret.SingleOrDefault( x => x.Id == s.Id ) );
         }
      }


      [TestMethod]
      public void OpenSprints_OrdersSprintsProperly()
      {
         var prevProject = "";
         var prevSortSeqNum = 0;
         var prevStartDate = DateTime.MinValue;

         var ret = _session.OpenSprints().ToList();
         foreach (var s in ret)
         {
            Assert.IsTrue( String.Compare( prevProject, s.Project.Name ) <= 0, "Projects out of order" );
            if (prevProject == s.Project.Name)
            {
               Assert.IsTrue( prevStartDate <= (s.StartDate ?? DateTime.MaxValue), "date out of order" );
               if (prevStartDate == s.StartDate)
               {
                  Assert.IsTrue( prevSortSeqNum < s.Status.SortSequence, "sequence out of order" );
               }
            }

            prevProject = s.Project.Name;
            prevStartDate = s.StartDate ?? DateTime.MaxValue;
            prevSortSeqNum = s.Status.SortSequence;
         }
      }


      [TestMethod]
      public void CurrentSprints_ReturnsSprintsThatAreActive_StartedTodayOrEarlier_NotPastEndDate()
      {
         var expectedSprints = Sprints.ModelData
            .Where( x => x.Status.StatusCd == 'A' && x.Status.Category == Domain.SprintStatusCategory.Active &&
                         x.StartDate != null && x.StartDate <= DateTime.Now.Date && (x.EndDate == null || x.EndDate >= DateTime.Now.Date) );

         var ret = _session.CurrentSprints().ToList();

         Assert.AreEqual( expectedSprints.Count(), ret.Count );
         foreach (var s in expectedSprints)
         {
            Assert.IsNotNull( ret.SingleOrDefault( x => x.Id == s.Id ) );
         }
      }


      [TestMethod]
      public void CurrentSprints_OrdersSprintsProperly()
      {
         var prevProject = "";
         var prevSortSeqNum = 0;
         var prevStartDate = DateTime.MinValue;

         var ret = _session.OpenSprints().ToList();
         foreach (var s in ret)
         {
            Assert.IsTrue( String.Compare( prevProject, s.Project.Name ) <= 0, "Projects out of order" );
            if (prevProject == s.Project.Name)
            {
               Assert.IsTrue( prevStartDate <= (s.StartDate ?? DateTime.MaxValue), "date out of order" );
               if (prevStartDate == s.StartDate)
               {
                  Assert.IsTrue( prevSortSeqNum < s.Status.SortSequence, "sequence out of order" );
               }
            }

            prevProject = s.Project.Name;
            prevStartDate = s.StartDate ?? DateTime.MaxValue;
            prevSortSeqNum = s.Status.SortSequence;
         }
      }
   }
}
