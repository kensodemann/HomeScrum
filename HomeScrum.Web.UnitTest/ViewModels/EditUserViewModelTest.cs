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
         Assert.IsNull( AttributeHelper.GetRequiredAttribute( typeof( EditUserViewModel ), "Password" ) );
      }

      public void ConfirmPassword_IsNotRequired()
      {
         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( typeof( EditUserViewModel ), "ConfirmPassword" ) );
      }
   }
}
