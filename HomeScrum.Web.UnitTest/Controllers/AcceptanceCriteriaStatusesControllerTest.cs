using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Common.TestData;
using System.Collections.Generic;
using System.Web.Mvc;
using HomeScrum.Data.Validators;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class AcceptanceCriteriaStatusesControllerTest : SystemObjectControllerTestBase<AcceptanceCriteriaStatus>
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
