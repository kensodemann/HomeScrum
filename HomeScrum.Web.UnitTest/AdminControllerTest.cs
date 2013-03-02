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
   public class AdminControllerTest
   {
      [TestMethod]
      public void WorkItemTypes_GetsAllWorkItemTypes()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemType>>();

         var controller = new AdminController( null, repository.Object );
         controller.WorkItemTypes();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void WorkItemTypes_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemType>>();

         repository.Setup( x => x.GetAll() )
            .Returns( WorkItemTypes.ModelData );

         var controller = new AdminController( null, repository.Object );
         var view = controller.WorkItemTypes() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }
   }
}
