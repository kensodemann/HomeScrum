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
   public class ProjectStatusesControllerTest
   {
      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<ProjectStatus>>();

         repository.Setup( x => x.GetAll() )
            .Returns( ProjectStatuses.ModelData );

         var controller = new ProjectStatusesController( repository.Object );
         var view = controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllProjectStatuses()
      {
         var repository = new Mock<IDataObjectRepository<ProjectStatus>>();

         var controller = new ProjectStatusesController( repository.Object );
         controller.Index();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }
   }
}
