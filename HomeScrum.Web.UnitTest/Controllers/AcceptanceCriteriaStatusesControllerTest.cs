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
   public class AcceptanceCriteriaStatusesControllerTest : SystemDataObjectControllerTestBase<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel>
   {
      protected override ICollection<AcceptanceCriterionStatus> GetAllModels()
      {
         return AcceptanceCriteriaStatuses.ModelData;
      }

      protected override AcceptanceCriterionStatus CreateNewModel()
      {
         return new AcceptanceCriterionStatus()
         {
            Name = "New Acceptance Criteria Status",
            Description = "New Acceptance Criteria Status",
            IsPredefined = false,
            IsAccepted = true,
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
         AcceptanceCriteriaStatuses.CreateTestModelData( initializeIds: true );
         _controller = new AcceptanceCriterionStatusesController( _validator.Object, new PropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         _controller.ControllerContext = new ControllerContext();
      }
   }
}
