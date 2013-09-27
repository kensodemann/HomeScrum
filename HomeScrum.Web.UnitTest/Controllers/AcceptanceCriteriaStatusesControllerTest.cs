using HomeScrum.Common.TestData;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
using HomeScrum.Web.Controllers.Base;
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
   public class AcceptanceCriteriaStatusesControllerTest : SystemDataObjectControllerTestBase<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel>
   {
      protected override ICollection<AcceptanceCriterionStatus> GetAllModels()
      {
         var models = _session.Query<AcceptanceCriterionStatus>()
            .OrderBy( x => x.SortSequence )
            .ToList();
         _session.Clear();

         return models;
      }

      protected override AcceptanceCriterionStatus CreateNewModel()
      {
         return new AcceptanceCriterionStatus()
         {
            Name = "New Acceptance Criteria Status",
            Description = "New Acceptance Criteria Status",
            IsPredefined = false,
            Category = AcceptanceCriterionStatusCategory.VerificationPassed,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         ReadWriteControllerTestBase<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         
         BuildDatabase();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }

      public override ReadWriteController<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel> CreateController()
      {
         var controller = new AcceptanceCriterionStatusesController( new PropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }

      private void BuildDatabase()
      {
         Database.Build( _session );
         Users.Load( _sessionFactory.Object );
         AcceptanceCriteriaStatuses.Load( _sessionFactory.Object );
      }

   }
}
