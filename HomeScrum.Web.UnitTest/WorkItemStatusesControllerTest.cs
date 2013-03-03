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
   public class WorkItemStatusesControllerTest
   {
      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemStatus>>();

         repository.Setup( x => x.GetAll() )
            .Returns( WorkItemStatuses.ModelData );

         var controller = new WorkItemStatusesController( repository.Object );
         var view = controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllWorkItemStatuses()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemStatus>>();

         var controller = new WorkItemStatusesController( repository.Object );
         controller.Index();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }
   }
}
