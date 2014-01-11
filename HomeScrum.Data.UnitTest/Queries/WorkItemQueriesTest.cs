using System;
using System.Linq;
using System.Security.Principal;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;

namespace HomeScrum.Data.UnitTest.Queries
{
   [TestClass]
   public class WorkItemQueriesTest
   {
      #region Test Initialization
      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;
      private User _user;

      [ClassInitialize]
      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         SetupSession();

         Database.Build( _session );
         WorkItems.Load( _sessionFactory.Object );

         SetupCurrentUser();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }


      private void SetupCurrentUser()
      {
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         _user = WorkItems.ModelData.First( x => x.AssignedToUser != null ).AssignedToUser;
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( _user.UserName );
      }


      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }
      #endregion


      [TestMethod]
      public void AllWorkItems_ReturnsAllWorkItems()
      {
         Assert.AreEqual( WorkItems.ModelData.Count(), _session.AllWorkItems().ToList().Count );
      }


      [TestMethod]
      public void AllWorkItems_SortsWorkItemsCorrectly()
      {
         var items = _session.AllWorkItems().ToList();
         var prevName = "";
         var prevStatusSortSeqNum = 0;
         var prevTypeSortSeqNum = 0;

         foreach (var item in items)
         {
            Assert.IsTrue( prevStatusSortSeqNum <= item.Status.SortSequence, "status sort" );
            if (prevStatusSortSeqNum == item.Status.SortSequence)
            {
               Assert.IsTrue( prevTypeSortSeqNum <= item.WorkItemType.SortSequence, "type sort" );
               if (prevTypeSortSeqNum == item.WorkItemType.SortSequence)
               {
                  Assert.IsTrue( String.Compare( prevName, item.Name, true ) < 0, "name sort" );
               }
            }
            prevName = item.Name;
            prevStatusSortSeqNum = item.Status.SortSequence;
            prevTypeSortSeqNum = item.WorkItemType.SortSequence;
         }
      }


      [TestMethod]
      public void Backlog_ReturnsAllBacklogItems()
      {
         var expected = WorkItems.ModelData.Where( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem );
         Assert.AreEqual( expected.Count(), _session.Backlog().ToList().Count );
      }


      [TestMethod]
      public void Backlog_SortsWorkItemsCorrectly()
      {
         var items = _session.Backlog().ToList();
         var prevName = "";
         var prevStatusSortSeqNum = 0;
         var prevTypeSortSeqNum = 0;

         foreach (var item in items)
         {
            Assert.IsTrue( prevStatusSortSeqNum <= item.Status.SortSequence, "status sort" );
            if (prevStatusSortSeqNum == item.Status.SortSequence)
            {
               Assert.IsTrue( prevTypeSortSeqNum <= item.WorkItemType.SortSequence, "type sort" );
               if (prevTypeSortSeqNum == item.WorkItemType.SortSequence)
               {
                  Assert.IsTrue( String.Compare( prevName, item.Name, true ) < 0, "name sort" );
               }
            }
            prevName = item.Name;
            prevStatusSortSeqNum = item.Status.SortSequence;
            prevTypeSortSeqNum = item.WorkItemType.SortSequence;
         }
      }


      [TestMethod]
      public void Backlog_ReturnsAllTaskItems()
      {
         var expected = WorkItems.ModelData.Where( x => x.WorkItemType.Category == WorkItemTypeCategory.Task );
         Assert.AreEqual( expected.Count(), _session.Tasks().ToList().Count );
      }


      [TestMethod]
      public void Tasks_SortsWorkItemsCorrectly()
      {
         var items = _session.Tasks().ToList();
         var prevName = "";
         var prevStatusSortSeqNum = 0;
         var prevTypeSortSeqNum = 0;

         foreach (var item in items)
         {
            Assert.IsTrue( prevStatusSortSeqNum <= item.Status.SortSequence, "status sort" );
            if (prevStatusSortSeqNum == item.Status.SortSequence)
            {
               Assert.IsTrue( prevTypeSortSeqNum <= item.WorkItemType.SortSequence, "type sort" );
               if (prevTypeSortSeqNum == item.WorkItemType.SortSequence)
               {
                  Assert.IsTrue( String.Compare( prevName, item.Name, true ) < 0, "name sort" );
               }
            }
            prevName = item.Name;
            prevStatusSortSeqNum = item.Status.SortSequence;
            prevTypeSortSeqNum = item.WorkItemType.SortSequence;
         }
      }


      [TestMethod]
      public void Problems_ReturnsAllIssueItems()
      {
         var expected = WorkItems.ModelData.Where( x => x.WorkItemType.Category == WorkItemTypeCategory.Issue );
         Assert.AreEqual( expected.Count(), _session.Problems().ToList().Count );
      }


      [TestMethod]
      public void Problems_SortsWorkItemsCorrectly()
      {
         var items = _session.Problems().ToList();
         var prevName = "";
         var prevStatusSortSeqNum = 0;
         var prevTypeSortSeqNum = 0;

         foreach (var item in items)
         {
            Assert.IsTrue( prevStatusSortSeqNum <= item.Status.SortSequence, "status sort" );
            if (prevStatusSortSeqNum == item.Status.SortSequence)
            {
               Assert.IsTrue( prevTypeSortSeqNum <= item.WorkItemType.SortSequence, "type sort" );
               if (prevTypeSortSeqNum == item.WorkItemType.SortSequence)
               {
                  Assert.IsTrue( String.Compare( prevName, item.Name, true ) < 0, "name sort" );
               }
            }
            prevName = item.Name;
            prevStatusSortSeqNum = item.Status.SortSequence;
            prevTypeSortSeqNum = item.WorkItemType.SortSequence;
         }
      }


      [TestMethod]
      public void Unassigned_FurtherLimitsTasksToThoseNotAssignedToUser()
      {
         var expected = WorkItems.ModelData.Where( x => x.WorkItemType.Category == WorkItemTypeCategory.Task && x.AssignedToUser == null );
         Assert.AreEqual( expected.Count(), _session.Tasks().Unassigned().ToList().Count );
      }


      [TestMethod]
      public void Unassigned_FurtherLimitsIssuesToThoseNotAssignedToUser()
      {
         var expected = WorkItems.ModelData.Where( x => x.WorkItemType.Category == WorkItemTypeCategory.Issue && x.AssignedToUser == null );
         Assert.AreEqual( expected.Count(), _session.Problems().Unassigned().ToList().Count );
      }


      [TestMethod]
      public void Unassigned_FurtherLimitsBacklogItemsToThoseNotAssignedToSprint()
      {
         var expected = WorkItems.ModelData.Where( x => x.WorkItemType.Category == WorkItemTypeCategory.BacklogItem && x.Sprint == null );
         Assert.AreEqual( expected.Count(), _session.Backlog().Unassigned().ToList().Count );
      }

      [TestMethod]
      public void Mine_LimitsToItemsAssignedToSpecifiedUser()
      {
         var expected = WorkItems.ModelData.Where( x => x.AssignedToUser == _user );
         Assert.AreEqual( expected.Count(), _session.AllWorkItems().Mine( _user.Id ).Count() );
      }
   }
}
