﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class AcceptanceCriteriaStatusTest
   {
      [TestMethod]
      public void IsAcceptedDisplayName()
      {
         var model = new AcceptanceCriteriaStatus();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsAccepted );

         Assert.IsNotNull( display, "Display attribute does not exist" );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "AcceptanceCriteriaStatusIsAccepted", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.AcceptanceCriteriaStatusIsAccepted ) );
      }
   }
}
