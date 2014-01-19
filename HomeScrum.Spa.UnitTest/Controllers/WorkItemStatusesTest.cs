using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.MockingKernel.Moq;
using Moq;
using Ninject.Extensions.Logging;
using System.Security.Principal;
using NHibernate;
using HomeScrum.Common.TestData;
using HomeScrum.Spa.Controllers;
using System.Linq;
using System.Web.Http;
using System.Net;
using HomeScrum.Data.Domain;

namespace HomeScrum.Spa.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemStatusesTest
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;
      private Mock<ILogger> _logger;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private WorkItemStatusesController _controller;

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

      private WorkItemStatusesController CreateController()
      {
         var controller = new WorkItemStatusesController( _logger.Object, _sessionFactory.Object );

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
         WorkItemStatuses.Load( _sessionFactory.Object );
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


      #region GET Tests
      [TestMethod]
      public void Get_ReturnsAllWorkItemStatuses()
      {
         var result = _controller.Get();

         Assert.AreEqual( WorkItemStatuses.ModelData.Count(), result.Count() );
         foreach (var item in result)
         {
            AssertItemsAreEqual( WorkItemStatuses.ModelData.Single( x => x.Id == item.Id ), item );
         }
      }

      [TestMethod]
      public void Get_RetunsWorkItemStatus_IdetifiedById()
      {
         var expected = WorkItemStatuses.ModelData[2];
         var result = _controller.Get( expected.Id.ToString() );

         AssertItemsAreEqual( expected, result );
      }


      [TestMethod]
      public void Get_RaisesNotFound_IfIdDoesNotExist()
      {
         try
         {
            var result = _controller.Get( Guid.NewGuid().ToString() );
            Assert.Fail();
         }
         catch (HttpResponseException e)
         {
            Assert.AreEqual( e.Response.StatusCode, HttpStatusCode.NotFound );
         }
      }

      [TestMethod]
      public void Get_RaisesNotFound_IfIdIsDefaultGuid()
      {
         try
         {
            var result = _controller.Get( Guid.Empty.ToString() );
            Assert.Fail();
         }
         catch (HttpResponseException e)
         {
            Assert.AreEqual( e.Response.StatusCode, HttpStatusCode.NotFound );
         }
      }

      [TestMethod]
      public void Get_RaisesNotFound_IfIdIsNotAGuid()
      {
         try
         {
            var result = _controller.Get( "NotAGuid" );
            Assert.Fail();
         }
         catch (HttpResponseException e)
         {
            Assert.AreEqual( e.Response.StatusCode, HttpStatusCode.NotFound );
         }
      }
      #endregion


      #region Private Helpers
      private void AssertItemsAreEqual( WorkItemStatus expected, WorkItemStatus actual )
      {
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
      }
      #endregion
   }
}
