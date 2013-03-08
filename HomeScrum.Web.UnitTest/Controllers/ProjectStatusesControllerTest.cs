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
   public class ProjectStatusesControllerTest : DataObjectBaseControllerTestBase<ProjectStatus>
   {
      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<ProjectStatus>>();
         _controller = new ProjectStatusesController( _repository.Object );
      }

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
            IsPredefined = 'N',
            IsActive = 'Y',
            StatusCd = 'A'
         };
      }
   }
}
