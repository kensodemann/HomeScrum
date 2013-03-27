using System;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class WorkItemStatusTest
   {
      [TestMethod]
      public void IsOpenDisplayName()
      {
         var model = new WorkItemStatus();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsOpenStatus );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "WorkItemStatusIsOpenStatus", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.WorkItemStatusIsOpenStatus ) );
      }
   }
}
