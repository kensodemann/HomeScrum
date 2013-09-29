using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Context;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class SprintStatusesControllerTest : SystemDataObjectControllerTestBase<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel>
   {
      protected override ICollection<SprintStatus> GetAllModels()
      {
         var models = _session.Query<SprintStatus>()
            .OrderBy( x => x.SortSequence )
            .ToList();

         _session.Clear();
         return models;
      }

      protected override SprintStatus CreateNewModel()
      {
         return new SprintStatus()
         {
            Name = "New Sprint Status",
            Description = "New Sprint Status",
            IsPredefined = false,
            Category = SprintStatusCategory.Active,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         ReadWriteControllerTestBase<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         
         Database.Build( _session );
         Users.Load( _sessionFactory.Object );
         SprintStatuses.Load( _sessionFactory.Object );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }

      public override Web.Controllers.Base.ReadWriteController<SprintStatus, SprintStatusEditorViewModel> CreateController()
      {
         var controller = new SprintStatusesController( new PropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }
   }
}
