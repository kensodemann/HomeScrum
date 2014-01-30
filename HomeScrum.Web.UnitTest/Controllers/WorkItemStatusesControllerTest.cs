using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemStatusesControllerTest : SystemDataObjectControllerTestBase<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>
   {
      protected override ICollection<WorkItemStatus> GetAllModels()
      {
         var models = _session.Query<WorkItemStatus>()
            .OrderBy( x => x.SortSequence )
            .ToList();

         _session.Clear();
         return models;
      }

      protected override WorkItemStatus CreateNewModel()
      {
         return new WorkItemStatus()
         {
            Name = "New Work Item Status",
            Description = "New Work Item Status",
            IsPredefined = false,
            Category = WorkItemStatusCategory.InProcess,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         ReadWriteControllerTestBase<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();

         Database.Build( _session );
         Users.Load( _sessionFactory.Object );
         WorkItemStatuses.Load( _sessionFactory.Object );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }

      public override ReadWriteController<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel> CreateController()
      {
         var controller = new WorkItemStatusesController( new PropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }
   }
}
