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
      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var repository = new Mock<IDataObjectRepository<AcceptanceCriteriaStatus>>();

         repository.Setup( x => x.GetAll() )
            .Returns( AcceptanceCriteriaStatuses.ModelData );

         var controller = new AcceptanceCriteriaStatusesController( repository.Object );
         var view = controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void AcceptanceCriteriaStatuses_GetsAllAcceptanceCriteriaStatuses()
      {
         var repository = new Mock<IDataObjectRepository<AcceptanceCriteriaStatus>>();

         var controller = new AcceptanceCriteriaStatusesController( repository.Object );
         controller.Index();

         repository.Verify( x => x.GetAll(), Times.Once() );
      }
   }
}
