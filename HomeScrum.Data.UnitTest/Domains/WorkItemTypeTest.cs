using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.UnitTest.Helpers;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class WorkItemTypeTest
   {
      [TestMethod]
      public void IsTaskDisplayName()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( WorkItemType ), "IsTask" );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.WorkItemTypeIsTask ) );
         Assert.AreEqual( "WorkItemTypeIsTask", display.Name );
      }
   }
}
