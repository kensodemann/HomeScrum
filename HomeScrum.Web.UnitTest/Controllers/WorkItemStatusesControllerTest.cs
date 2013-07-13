using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemStatusesControllerTest : SystemDataObjectControllerTestBase<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>
   {
      protected override ICollection<WorkItemStatus> GetAllModels()
      {
         return WorkItemStatuses.ModelData;
      }

      protected override WorkItemStatus CreateNewModel()
      {
         return new WorkItemStatus()
         {
            Name = "New Work Item Status",
            Description = "New Work Item Status",
            IsPredefined = false,
            IsOpenStatus = true,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         WorkItemStatuses.CreateTestModelData( initializeIds: true );
         _controller = new WorkItemStatusesController( _repository.Object, _validator.Object, new PropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         _controller.ControllerContext = new ControllerContext();
      }
   }
}
