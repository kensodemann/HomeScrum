using HomeScrum.Data.Common.Test.Utility;
using HomeScrum.Web.Models.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
