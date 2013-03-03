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
   public class AcceptanceCriteriaStatusesControllerTest
   {
      private Mock<IDataObjectRepository<AcceptanceCriteriaStatus>> repository;

      [TestInitialize]
      public void InitializeTest()
      {
         repository = new Mock<IDataObjectRepository<AcceptanceCriteriaStatus>>();
      }


      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         repository.Setup( x => x.GetAll() )
            .Returns( AcceptanceCriteriaStatuses.ModelData );

         var controller = new AcceptanceCriteriaStatusesController( repository.Object );
         var view = controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllAcceptanceCriteriaStatuses()
      {
         var controller = new AcceptanceCriteriaStatusesController( repository.Object );
         controller.Index();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = AcceptanceCriteriaStatuses.ModelData[2];

         repository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var controller = new AcceptanceCriteriaStatusesController( repository.Object );
         var view = controller.Details( model.Id ) as ViewResult;

         repository.Verify( x => x.Get( model.Id ), Times.Once() );

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.AreEqual( model, view.Model );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         repository.Setup( x => x.Get( id ) ).Returns( null as AcceptanceCriteriaStatus );

         var controller = new AcceptanceCriteriaStatusesController( repository.Object );
         var result = controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

   }
}
