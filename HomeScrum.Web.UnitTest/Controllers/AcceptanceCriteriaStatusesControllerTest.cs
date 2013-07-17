using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Common.Utility;
using HomeScrum.Web.Controllers.Admin;


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
         Database.Initialize();
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         Database.Build();
         AcceptanceCriteriaStatuses.Load();
         base.InitializeTest();
      }

      public override ReadWriteController<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel> CreateDatabaseConnectedController()
      {
         var controller = new AcceptanceCriterionStatusesController( _validator.Object, new PropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>(), _logger.Object, NHibernateHelper.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }

      public override ReadWriteController<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel> CreateDatabaseMockedController()
      {
         var controller = new AcceptanceCriterionStatusesController( _validator.Object, new PropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
