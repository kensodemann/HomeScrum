using System;
using HomeScrum.Data.Domain;
using HomeScrum.Data.UnitTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class UserTest
   {
      [TestMethod]
      public void UserIdDisplayNameAndPrompt()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( User ), "UserId" );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "UserId", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.UserId ) );
         Assert.AreEqual( "UserIdPrompt", display.Prompt );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.UserIdPrompt ) );
      }

      [TestMethod]
      public void UserIdIsRequired()
      {
         var required = AttributeHelper.GetRequiredAttribute( typeof( User ), "UserId" );

         Assert.IsNotNull( required );
         Assert.AreEqual( "UserIdIsRequired", required.ErrorMessageResourceName );
         Assert.AreEqual( typeof( ErrorMessages ), required.ErrorMessageResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( ErrorMessages.UserIdIsRequired ) );
      }

      [TestMethod]
      public void FirstNameDisplayNameAndPrompt()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( User ), "FirstName" );

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
         var required = AttributeHelper.GetRequiredAttribute( typeof( User ), "FirstName" );

         Assert.IsNotNull( required );
         Assert.AreEqual( "FirstNameIsRequired", required.ErrorMessageResourceName );
         Assert.AreEqual( typeof( ErrorMessages ), required.ErrorMessageResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( ErrorMessages.FirstNameIsRequired ) );
      }

      [TestMethod]
      public void LastNameDisplayNameAndPrompt()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( User ), "LastName" );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "LastName", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.LastName ) );
         Assert.IsTrue( String.IsNullOrWhiteSpace( display.Prompt ) );
      }

      [TestMethod]
      public void LastNameIsNotRequired()
      {
         var required = AttributeHelper.GetRequiredAttribute( typeof( User ), "LastName" );

         Assert.IsNull( required );
      }

      [TestMethod]
      public void MiddleNameDisplayNameAndPrompt()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( User ), "MiddleName" );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "MiddleName", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.MiddleName ) );
         Assert.IsTrue( String.IsNullOrWhiteSpace( display.Prompt ) );
      }

      [TestMethod]
      public void MiddleNameIsNotRequired()
      {
         var required = AttributeHelper.GetRequiredAttribute( typeof( User ), "MiddleName" );

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
         var display = AttributeHelper.GetDisplayAttribute( typeof( User ), "IsActive" );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "UserIsActive", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.UserIsActive ) );
      }
   }
}
