using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Common.TestData;
using System.Collections.Generic;
using System.Web.Mvc;


namespace HomeScrum.Web.UnitTest
{
   [TestClass]
   public class SprintStatusesControllerTest : DataObjectBaseControllerTestBase<SprintStatus>
   {
      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<SprintStatus>>();
         _controller = new SprintStatusesController( _repository.Object );
      }

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
            IsPredefined = 'N',
            IsOpenStatus = 'Y',
            StatusCd = 'A'
         };
      }
   }
}
