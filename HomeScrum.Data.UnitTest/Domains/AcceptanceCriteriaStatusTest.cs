using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.UnitTest.Helpers;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class AcceptanceCriteriaStatusTest
   {
      [TestMethod]
      public void IsAcceptedDisplayName()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( AcceptanceCriteriaStatus ), "IsAccepted" );

         Assert.IsNotNull( display, "Display attribute does not exist" );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "AcceptanceCriteriaStatusIsAccepted", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.AcceptanceCriteriaStatusIsAccepted ) );
      }
   }
}
