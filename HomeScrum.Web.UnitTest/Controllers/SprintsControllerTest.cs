using System;
using HomeScrum.Common.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using System.Collections.Generic;
using HomeScrum.Web.Models.Base;
using System.Linq;
using HomeScrum.Web.Controllers;
using Ninject.Extensions.Logging;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Translators;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class SprintsControllerTest
   {
      #region Test Setup
      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private Mock<ILogger> _logger;

      private SprintsController _controller;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         BuildMocks();
         SetupSession();
         BuildDatabase();

         CreateController();
      }

      private void BuildMocks()
      {
         _sessionFactory = new Mock<ISessionFactory>();
         _logger = new Mock<ILogger>();
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      private void BuildDatabase()
      {
         Database.Build( _session );
         Sprints.Load( _sessionFactory.Object );
      }

      private void CreateController()
      {
         _controller = new SprintsController( new PropertyNameTranslator<Sprint, SprintEditorViewModel>(), _logger.Object, _sessionFactory.Object );
      }


      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion

      [TestMethod]
      public void Index_ReturnsViewWithAllItems()
      {
         var view = _controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<DomainObjectViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( Sprints.ModelData.Count(), model.Count() );

         foreach (var sprint in Sprints.ModelData)
         {
            Assert.IsNotNull( model.FirstOrDefault( x => x.Id == sprint.Id ) );
         }
      }
   }
}
