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
   public class WorkItemStatusesControllerTest : DataObjectBaseControllerTestBase<WorkItemStatus>
   {
      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<WorkItemStatus>>();
         _controller = new WorkItemStatusesController( _repository.Object );
      }

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
            IsOpenStatus = 'Y',
            StatusCd = 'A'
         };
      }
   }
}
