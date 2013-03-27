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
   public class ProjectStatusesControllerTest : DataObjectBaseControllerTestBase<ProjectStatus>
   {
      protected override ICollection<ProjectStatus> GetAllModels()
      {
         return ProjectStatuses.ModelData;
      }

      protected override ProjectStatus CreateNewModel()
      {
         return new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            IsPredefined = false,
            IsActive = true,
            AllowUse = true
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         ProjectStatuses.CreateTestModelData();
         _controller = new ProjectStatusesController( _repository.Object, _validator.Object );
      }
   }
}
