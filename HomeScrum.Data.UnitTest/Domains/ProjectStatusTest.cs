using System;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectStatusTest
   {
      [TestMethod]
      public void IsActiveDisplayName()
      {
         var model = new ProjectStatus();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsActive );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "ProjectStatusIsActive", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.ProjectStatusIsActive ) );
      }
   }
}
