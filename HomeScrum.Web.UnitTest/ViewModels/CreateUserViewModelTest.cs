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

         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( () => model.NewPassword ) );
      }

      [TestMethod]
      public void Password_MinStringLength()
      {
         var model = new CreateUserViewModel();
         var stringLengthAttribute = AttributeHelper.GetStringLengthAttribute( () => model.NewPassword );

         Assert.IsNotNull( stringLengthAttribute );
         Assert.AreEqual( 6, stringLengthAttribute.MinimumLength );
         Assert.AreEqual( 100, stringLengthAttribute.MaximumLength );
      }

      [TestMethod]
      public void Password_Display()
      {
         var model = new CreateUserViewModel();
         var displayAttribute = AttributeHelper.GetDisplayAttribute( () => model.NewPassword );

         Assert.IsNotNull( displayAttribute );
         Assert.AreEqual( "Password:", displayAttribute.Name );
      }


      [TestMethod]
      public void ConfirmPassword_IsRequired()
      {
         var model = new CreateUserViewModel();

         Assert.IsNotNull( AttributeHelper.GetRequiredAttribute( () => model.ConfirmPassword ) );
      }

      [TestMethod]
      public void ConfirmPassword_ComparedToPassword()
      {
         var model = new CreateUserViewModel();
         var compareAttribute = AttributeHelper.GetCompareAttribute( () => model.ConfirmPassword );

         Assert.IsNotNull( compareAttribute );
         Assert.AreEqual( "NewPassword", compareAttribute.OtherProperty );
      }

      [TestMethod]
      public void ConfirmPassword_Display()
      {
         var model = new CreateUserViewModel();
         var displayAttribute = AttributeHelper.GetDisplayAttribute( () => model.ConfirmPassword );

         Assert.IsNotNull( displayAttribute );
         Assert.AreEqual( "Confirm password:", displayAttribute.Name );
      }
   }
}
