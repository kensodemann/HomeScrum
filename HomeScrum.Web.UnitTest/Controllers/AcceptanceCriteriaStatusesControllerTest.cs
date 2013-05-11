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
   public class AcceptanceCriteriaStatusesControllerTest : ReadWriteControllerTestBase<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusViewModel, AcceptanceCriteriaStatusEditorViewModel>
   {
      protected override ICollection<AcceptanceCriteriaStatus> GetAllModels()
      {
         return AcceptanceCriteriaStatuses.ModelData;
      }

      protected override AcceptanceCriteriaStatus CreateNewModel()
      {
         return new AcceptanceCriteriaStatus()
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
         _controller = new AcceptanceCriteriaStatusesController( _repository.Object, _validator.Object, new GenericTranslator<AcceptanceCriteriaStatusEditorViewModel>() );
         _controller.ControllerContext = new ControllerContext();
      }
   }
}
