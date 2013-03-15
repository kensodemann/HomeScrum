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
   public class WorkItemStatusesControllerTest : DataObjectBaseControllerTestBase<WorkItemStatus>
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
            AllowUse = true
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         _controller = new WorkItemStatusesController( _repository.Object, _validator.Object );
      }
   }
}
