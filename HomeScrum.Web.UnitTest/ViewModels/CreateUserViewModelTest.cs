using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Models;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class CreateUserViewModelTest
   {
      [TestMethod]
      public void CreateUser_IsNewUser()
      {
         var model = new CreateUserViewModel();

         Assert.IsTrue( model.IsNewUser );
      }
   }
}
