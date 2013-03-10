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
         Assert.IsNotNull( display.ResourceType );
         Assert.AreEqual( "Name", display.Name );
         Assert.AreEqual( "NamePrompt", display.Prompt );
      }

      [TestMethod]
      public void DescriptionDisplayNameAndPrompt()
      {
         var display = AttributeHelper.GetDisplayAttribute( typeof( DataObjectBase ), "Description" );

         Assert.IsNotNull( display );
         Assert.IsNotNull( display.ResourceType );
         Assert.AreEqual( "Description", display.Name );
         Assert.AreEqual( "DescriptionPrompt", display.Prompt );
      }
   }
}
