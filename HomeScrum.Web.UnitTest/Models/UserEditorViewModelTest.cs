using System;
using HomeScrum.Common.TestData;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Web.UnitTest.Models
{
   [TestClass]
   public class UserEditorViewModelTest
   {
      [TestMethod]
      public void UserIsNew_WhenDefaultConstructorUsed()
      {
         var viewModel = new UserEditorViewModel();
         Assert.IsTrue( viewModel.UserIsNew );
      }

      [TestMethod]
      public void UserIsNew_WhenUserConstructorUsed()
      {
         var viewModel = new UserEditorViewModel( Users.ModelData[0] );
         Assert.IsFalse( viewModel.UserIsNew );
      }
   }
}
