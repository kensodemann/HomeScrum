using System;
using HomeScrum.Common.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class SprintsControllerTest
   {
      #region Test Setup
      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
      }
      
      [TestInitialize]
      public void InitializeTest()
      {
         SetupSession();
         BuildDatabase();
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      private void BuildDatabase()
      {
         Database.Build( _session );
         Users.Load( _sessionFactory.Object );
         SprintStatuses.Load( _sessionFactory.Object );
         
      }


      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion

      [TestMethod]
      public void TestMethod1()
      {
      }
   }
}
