using System;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class UserTest
   {
      [TestMethod]
      public void UserNameDisplayNameAndPrompt()
      {
         var model = new User();

         var display = AttributeHelper.GetDisplayAttribute( () => model.UserName );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "UserName", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.UserName ) );
         Assert.AreEqual( "UserNamePrompt", display.Prompt );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.UserNamePrompt ) );
      }

      [TestMethod]
      public void UserNameRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.UserName );

         Assert.IsNotNull( required );
         Assert.AreEqual( "UserNameIsRequired", required.ErrorMessageResourceName );
         Assert.AreEqual( typeof( ErrorMessages ), required.ErrorMessageResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( ErrorMessages.UserNameIsRequired ) );
      }

      [TestMethod]
      public void FirstNameDisplayNameAndPrompt()
      {
         var model = new User();

         var display = AttributeHelper.GetDisplayAttribute( () => model.FirstName );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "FirstName", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.FirstName ) );
         Assert.AreEqual( "FirstNamePrompt", display.Prompt );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.FirstNamePrompt ) );
      }

      [TestMethod]
      public void FirstNameIsRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.FirstName );

         Assert.IsNotNull( required );
         Assert.AreEqual( "FirstNameIsRequired", required.ErrorMessageResourceName );
         Assert.AreEqual( typeof( ErrorMessages ), required.ErrorMessageResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( ErrorMessages.FirstNameIsRequired ) );
      }

      [TestMethod]
      public void LastNameDisplayNameAndPrompt()
      {
         var model = new User();

         var display = AttributeHelper.GetDisplayAttribute( () => model.LastName );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "LastName", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.LastName ) );
         Assert.IsTrue( String.IsNullOrWhiteSpace( display.Prompt ) );
      }

      [TestMethod]
      public void LastNameIsNotRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.LastName );

         Assert.IsNull( required );
      }

      [TestMethod]
      public void MiddleNameDisplayNameAndPrompt()
      {
         var model = new User();

         var display = AttributeHelper.GetDisplayAttribute( () => model.MiddleName );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "MiddleName", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.MiddleName ) );
         Assert.IsTrue( String.IsNullOrWhiteSpace( display.Prompt ) );
      }

      [TestMethod]
      public void MiddleNameIsNotRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.MiddleName );

         Assert.IsNull( required );
      }

      [TestMethod]
      public void StatusCdIsI_IfUserNotActive()
      {
         var model = new User();

         model.IsActive = false;

         Assert.AreEqual( 'I', model.StatusCd );
         Assert.IsFalse( model.IsActive );
      }

      [TestMethod]
      public void StatusCdIsA_IfUserIsActive()
      {
         var model = new User();

         model.IsActive = true;

         Assert.AreEqual( 'A', model.StatusCd );
         Assert.IsTrue( model.IsActive );
      }

      [TestMethod]
      public void IsActiveName()
      {
         var model = new User();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsActive );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "UserIsActive", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.UserIsActive ) );
      }
   }
}
