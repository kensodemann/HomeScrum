using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.UnitTest.Helpers;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class DataObjectBaseTest
   {
      [TestMethod]
      public void NameDisplayNameAndPrompt()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( DataObjectBase ), "Name" );

         Assert.IsNotNull( display );
         Assert.AreEqual( "Name", display.Name );
         Assert.AreEqual( "Enter a unique name", display.Prompt );
      }

      [TestMethod]
      public void DescriptionDisplayNameAndPrompt()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( DataObjectBase ), "Description" );

         Assert.IsNotNull( display );
         Assert.AreEqual( "Description", display.Name );
         Assert.AreEqual( "Enter a short description", display.Prompt );
      }
   }
}
