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
   public class SprintStatusesControllerTest : DataObjectBaseControllerTestBase<SprintStatus>
   {
      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<SprintStatus>>();
         _validator = new Mock<IValidator<SprintStatus>>();
         _controller = new SprintStatusesController( _repository.Object, _validator.Object );
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
            IsPredefined = false,
            IsOpenStatus = true,
            AllowUse = true
         };
      }
   }
}
