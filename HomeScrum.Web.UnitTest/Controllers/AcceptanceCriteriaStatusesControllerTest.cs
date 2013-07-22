using HomeScrum.Common.TestData;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
         using (var session = NHibernateHelper.OpenSession())
         {
            return session.Query<AcceptanceCriterionStatus>()
               .OrderBy( x => x.SortSequence )
               .ToList();
         }
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
         Users.Load();
         AcceptanceCriteriaStatuses.Load();
         base.InitializeTest();
      }

      public override ReadWriteController<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel> CreateController()
      {
         var controller = new AcceptanceCriterionStatusesController( new PropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>(), _logger.Object, NHibernateHelper.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
