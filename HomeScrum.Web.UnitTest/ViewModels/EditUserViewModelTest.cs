using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Models;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class EditUserViewModelTest
   {
      [TestMethod]
      public void EditUser_IsNotNewUser()
      {
         var model = new EditUserViewModel();

         Assert.IsFalse( model.IsNewUser );
      }

      [TestMethod]
      public void Password_IsNotRequired()
      {
         var model = new EditUserViewModel();

         Assert.IsNull( AttributeHelper.GetRequiredAttribute( () => model.NewPassword ) );
      }

      public void ConfirmPassword_IsNotRequired()
      {
         var model = new EditUserViewModel();

         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( () => model.ConfirmPassword ) );
      }
   }
}
