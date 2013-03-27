using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Models;

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
   }
}
