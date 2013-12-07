using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.MockingKernel.Moq;
using Moq;
using Ninject.Extensions.Logging;
using System.Security.Principal;
using NHibernate;
using HomeScrum.Common.TestData;
using HomeScrum.Spa.Controllers;

namespace HomeScrum.Spa.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemTypesTest
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;
      private Mock<ILogger> _logger;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private WorkItemTypesController _controller;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         SetupSession();
         CreateMockIOCKernel();

         BuildDatabase();
         SetupLogger();

         _controller = CreateController();
      }

      private WorkItemTypesController CreateController()
      {
         var controller = new WorkItemTypesController( _logger.Object, _sessionFactory.Object );

         return controller;
      }

      private void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( _sessionFactory.Object );
      }

      private void BuildDatabase()
      {
         Database.Build( _session );
         WorkItemTypes.Load( _sessionFactory.Object );
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
