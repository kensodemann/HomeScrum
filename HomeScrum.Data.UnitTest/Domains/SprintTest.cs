using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Moq;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class SprintTest
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
         SetupSession();

         Database.Build( _session );
         Sprints.Load( _sessionFactory.Object );
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
      public void Sprint_CanGet()
      {
         var id = Sprints.ModelData[1].Id;

         var sprint = _session.Get<Sprint>( id );

         Assert.IsNotNull( sprint );
         Assert.AreEqual( id, sprint.Id );
         Assert.AreEqual( Sprints.ModelData[1].Name, sprint.Name );
         Assert.AreEqual( Sprints.ModelData[1].Description, sprint.Description );

         Assert.IsNotNull( sprint.Status );
         Assert.IsNotNull( sprint.Project );
      }
   }
}
