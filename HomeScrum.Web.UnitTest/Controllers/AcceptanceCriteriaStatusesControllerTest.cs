using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class AcceptanceCriteriaStatusesControllerTest : DomainObjectControllerTestBase<AcceptanceCriteriaStatus, AcceptanceCriteriaStatusEditorViewModel>
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
            AllowUse = true
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         AcceptanceCriteriaStatuses.CreateTestModelData();
         _controller = new AcceptanceCriteriaStatusesController( _repository.Object, _validator.Object );
      }



   }
}
