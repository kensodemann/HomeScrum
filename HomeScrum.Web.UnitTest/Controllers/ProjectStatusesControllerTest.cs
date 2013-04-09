using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class ProjectStatusesControllerTest : ReadWriteControllerTestBase<ProjectStatus>
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
            StatusCd = 'A'
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         _controller = new ProjectStatusesController( _repository.Object, _validator.Object );
      }
   }
}
