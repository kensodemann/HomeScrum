using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Common.TestData;
using System.Collections.Generic;
using System.Web.Mvc;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class AcceptanceCriteriaStatusesControllerTest : DataObjectBaseControllerTestBase<AcceptanceCriteriaStatus>
   {
      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<AcceptanceCriteriaStatus>>();
         _controller = new AcceptanceCriteriaStatusesController( _repository.Object );
      }

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
            IsPredefined = 'N',
            IsAccepted = 'Y',
            StatusCd = 'A'
         };
      }
   }
}
