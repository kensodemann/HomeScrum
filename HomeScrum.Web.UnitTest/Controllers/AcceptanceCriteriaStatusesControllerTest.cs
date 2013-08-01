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
         using (var session = Database.OpenSession())
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
         ReadWriteControllerTestBase<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         CurrentSessionContext.Bind( Database.SessionFactory.OpenSession() );
         BuildDatabase();
         base.InitializeTest();
      }
      
      [TestCleanup]
      public void CleanupTest()
      {
         var session = CurrentSessionContext.Unbind( Database.SessionFactory );
         session.Dispose();
      }

      public override ReadWriteController<AcceptanceCriterionStatus, AcceptanceCriterionStatusViewModel, AcceptanceCriterionStatusEditorViewModel> CreateController()
      {
         var controller = new AcceptanceCriterionStatusesController( new PropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }

      private static void BuildDatabase()
      {
         Database.Build();
         Users.Load();
         AcceptanceCriteriaStatuses.Load();
      }

   }
}
