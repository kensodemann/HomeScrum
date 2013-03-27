using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Models;
using HomeScrum.Data.Common.Test.Utility;

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

      [TestMethod]
      public void Password_IsRequired()
      {
         var model = new CreateUserViewModel();

         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( () => model.Password ) );
      }

      [TestMethod]
      public void ConfirmPassword_IsRequired()
      {
         var model = new CreateUserViewModel();

         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( () => model.ConfirmPassword ) );
      }
   }
}
