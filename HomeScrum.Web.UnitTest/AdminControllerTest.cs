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
      public void SprintStatuses_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<SprintStatus>>();

         repository.Setup( x => x.GetAll() )
            .Returns( SprintStatuses.ModelData );

         var controller = new AdminController( null, null, repository.Object, null, null );
         var view = controller.SprintStatuses() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void SprintStatuses_GetsAllSprintStatuses()
      {
         var repository = new Mock<IDataObjectRepository<SprintStatus>>();

         var controller = new AdminController( null, null, repository.Object, null, null );
         controller.SprintStatuses();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void WorkItemStatuses_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemStatus>>();

         repository.Setup( x => x.GetAll() )
            .Returns( WorkItemStatuses.ModelData );

         var controller = new AdminController( null, null, null, repository.Object, null );
         var view = controller.WorkItemStatuses() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void WorkItemStatuses_GetsAllWorkItemStatuses()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemStatus>>();

         var controller = new AdminController( null, null, null, repository.Object, null );
         controller.WorkItemStatuses();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void WorkItemTypes_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemType>>();

         repository.Setup( x => x.GetAll() )
            .Returns( WorkItemTypes.ModelData );

         var controller = new AdminController( null, null, null, null, repository.Object );
         var view = controller.WorkItemTypes() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void WorkItemTypes_GetsAllWorkItemTypes()
      {
         var repository = new Mock<IDataObjectRepository<WorkItemType>>();

         var controller = new AdminController( null, null, null, null, repository.Object );
         controller.WorkItemTypes();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }
   }
}
