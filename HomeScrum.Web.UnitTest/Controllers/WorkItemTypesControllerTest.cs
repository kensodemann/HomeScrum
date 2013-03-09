using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Common.TestData;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemTypeesControllerTest : DataObjectBaseControllerTestBase<WorkItemType>
   {
      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<WorkItemType>>();
         _controller = new WorkItemTypesController( _repository.Object );
      }

      protected override ICollection<WorkItemType> GetAllModels()
      {
         return WorkItemTypes.ModelData;
      }

      protected override WorkItemType CreateNewModel()
      {
         return new WorkItemType()
         {
            Name = "New Work Item Type",
            Description = "New Work Item Type",
            IsPredefined = false,
            IsTask = false,
            AllowUse = true
         };
      }
   }
}
