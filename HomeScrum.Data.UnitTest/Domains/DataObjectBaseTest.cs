using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class DataObjectBaseTest
   {
      [TestMethod]
      public void NameDisplayNameAndPrompt()
      {
         var model = new DataObjectBase();

         var display = AttributeHelper.GetDisplayAttribute( () => model.Name );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "Name", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.Name ) );
         Assert.AreEqual( "NamePrompt", display.Prompt );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.NamePrompt ) );
      }

      [TestMethod]
      public void NameIsRequired()
      {
         var model = new DataObjectBase();

         var required = AttributeHelper.GetRequiredAttribute( () => model.Name );

         Assert.IsNotNull( required );
         Assert.AreEqual( "NameIsRequired", required.ErrorMessageResourceName );
         Assert.AreEqual( typeof( ErrorMessages ), required.ErrorMessageResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( ErrorMessages.NameIsRequired ) );
      }

      [TestMethod]
      public void DescriptionDisplayNameAndPrompt()
      {
         var model = new DataObjectBase();

         var display = AttributeHelper.GetDisplayAttribute( () => model.Description );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "Description", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.Description ) );
         Assert.AreEqual( "DescriptionPrompt", display.Prompt );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.DescriptionPrompt ) );
      }
   }
}
