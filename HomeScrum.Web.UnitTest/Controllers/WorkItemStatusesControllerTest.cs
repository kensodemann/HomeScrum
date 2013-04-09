using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemStatusesControllerTest : DomainObjectControllerTestBase<WorkItemStatus>
   {
      protected override ICollection<WorkItemStatus> GetAllModels()
      {
         return WorkItemStatuses.ModelData;
      }

      protected override WorkItemStatus CreateNewModel()
      {
         return new WorkItemStatus()
         {
            Name = "New Work Item Status",
            Description = "New Work Item Status",
            IsPredefined = false,
            IsOpenStatus = true,
            StatusCd = 'A'
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         WorkItemStatuses.CreateTestModelData( initializeIds: true );
         _controller = new WorkItemStatusesController( _repository.Object, _validator.Object );
      }
   }
}
