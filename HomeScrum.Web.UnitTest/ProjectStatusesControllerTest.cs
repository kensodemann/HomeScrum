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
      private Mock<IDataObjectRepository<ProjectStatus>> _repository;
      private ProjectStatusesController _controller;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<ProjectStatus>>();
         _controller = new ProjectStatusesController( _repository.Object );
      }


      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _repository.Setup( x => x.GetAll() )
            .Returns( ProjectStatuses.ModelData );

         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllProjectStatuses()
      {
         _controller.Index();

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = ProjectStatuses.ModelData[2];

         _repository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var view = _controller.Details( model.Id ) as ViewResult;

         _repository.Verify( x => x.Get( model.Id ), Times.Once() );

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.AreEqual( model, view.Model );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         _repository.Setup( x => x.Get( id ) ).Returns( null as ProjectStatus );

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }
   }
}
