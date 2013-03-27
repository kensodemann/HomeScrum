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
         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( typeof( CreateUserViewModel ), "Password" ) );
      }

      [TestMethod]
      public void ConfirmPassword_IsRequired()
      {
         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( typeof( CreateUserViewModel ), "ConfirmPassword" ) );
      }
   }
}
