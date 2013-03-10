using System;
using HomeScrum.Data.Domain;
using HomeScrum.Data.UnitTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectStatusTest
   {
      [TestMethod]
      public void IsActiveDisplayName()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( ProjectStatus ), "IsActive" );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "ProjectStatusIsActive", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.ProjectStatusIsActive ) );
      }
   }
}
