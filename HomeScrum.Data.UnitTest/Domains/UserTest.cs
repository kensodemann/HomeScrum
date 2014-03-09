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
      public void UserNameRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.UserName );

         Assert.IsNotNull( required );
      }

      [TestMethod]
      public void FirstNameIsRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.FirstName );

         Assert.IsNotNull( required );
      }

      [TestMethod]
      public void LastNameIsNotRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.LastName );

         Assert.IsNull( required );
      }

      [TestMethod]
      public void MiddleNameIsNotRequired()
      {
         var model = new User();

         var required = AttributeHelper.GetRequiredAttribute( () => model.MiddleName );

         Assert.IsNull( required );
      }
   }
}
