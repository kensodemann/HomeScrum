using System;
using HomeScrum.Data.Domain;
using HomeScrum.Data.UnitTest.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class SprintStatusTest
   {
      [TestMethod]
      public void IsOpenDisplayName()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( SprintStatus ), "IsOpenStatus" );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "SprintStatusIsOpenStatus", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.SprintStatusIsOpenStatus ) );
      }
   }
}
