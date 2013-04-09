using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class SprintStatusesControllerTest : DomainObjectControllerTestBase<SprintStatus>
   {
      protected override ICollection<SprintStatus> GetAllModels()
      {
         return SprintStatuses.ModelData;
      }

      protected override SprintStatus CreateNewModel()
      {
         return new SprintStatus()
         {
            Name = "New Sprint Status",
            Description = "New Sprint Status",
            IsPredefined = false,
            IsOpenStatus = true,
            StatusCd = 'A'
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         SprintStatuses.CreateTestModelData( initializeIds: true );
         _controller = new SprintStatusesController( _repository.Object, _validator.Object );
      }
   }
}
