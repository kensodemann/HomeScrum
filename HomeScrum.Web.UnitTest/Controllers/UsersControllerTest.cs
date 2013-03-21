using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using System.Web.Mvc;
using HomeScrum.Common.TestData;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class UsersControllerTest
   {
      private Mock<IRepository<User>> _repository;
      private Mock<IValidator<User>> _validator;
      private UsersController _controller;

      [TestInitialize]
      public virtual void InitializeTest()
      {
         _repository = new Mock<IRepository<User>>();
         _validator = new Mock<IValidator<User>>();

         _validator.Setup( x => x.ModelIsValid( It.IsAny<User>() ) ).Returns( true );

         _controller = new UsersController( _repository.Object );
      }


      [TestMethod]
      public void Index_GetsAllItems()
      {
         _controller.Index();

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _repository.Setup( x => x.GetAll() )
            .Returns( Users.ModelData );

         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }
   }
}
