using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class AccountControllerTest
   {
      private Mock<ISecurityRepository> _securityRepository;
      private AccountController _controller;

      [TestInitialize]
      public void InitializeTest()
      {
         _securityRepository = new Mock<ISecurityRepository>();
         _controller = new AccountController();
      }
      
      [TestMethod]
      public void LoginGet_ReturnsView()
      {
         var result = _controller.Login( "/something" ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( "/something", result.ViewBag.ReturnAddress );
      }
   }
}
