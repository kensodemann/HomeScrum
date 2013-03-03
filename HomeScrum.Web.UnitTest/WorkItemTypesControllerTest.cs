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
   public class WorkItemTypeesControllerTest
   {
      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemType>>();

         repository.Setup( x => x.GetAll() )
            .Returns( WorkItemTypes.ModelData );

         var controller = new WorkItemTypesController( repository.Object );
         var view = controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllWorkItemTypes()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemType>>();

         var controller = new WorkItemTypesController( repository.Object );
         controller.Index();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }
   }
}
