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
      private Mock<IDataObjectRepository<WorkItemType>> _repository;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<WorkItemType>>();
      }


      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _repository.Setup( x => x.GetAll() )
            .Returns( WorkItemTypes.ModelData );

         var controller = new WorkItemTypesController( _repository.Object );
         var view = controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllWorkItemTypes()
      {
         var controller = new WorkItemTypesController( _repository.Object );
         controller.Index();

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = WorkItemTypes.ModelData[2];

         _repository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var controller = new WorkItemTypesController( _repository.Object );
         var view = controller.Details( model.Id ) as ViewResult;

         _repository.Verify( x => x.Get( model.Id ), Times.Once() );

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.AreEqual( model, view.Model );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         _repository.Setup( x => x.Get( id ) ).Returns( null as WorkItemType );

         var controller = new WorkItemTypesController( _repository.Object );
         var result = controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }
   }
}
