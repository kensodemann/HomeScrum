using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Translators;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.UnitTest.Translators
{
   [TestClass]
   public class PropertyNameTranslatorTest
   {
      class TestClass
      {
         public Guid Id { get; set; }
         public string Name { get; set; }
         public string Description { get; set; }
         public int GrossMargin { get; set; }
      }

      private PropertyNameTranslator _translator;
      private TestClass _testObject;

      [TestInitialize]
      public void InitializeTest()
      {
         BuildTestObjects();
         SetupTranslator();
      }

      private void BuildTestObjects()
      {
         _translator = new PropertyNameTranslator();
         _testObject = new TestClass();
      }

      private void SetupTranslator()
      {
         _translator.AddTranslation( "GrossMargin", "Profit" );
         _translator.AddTranslation( () => _testObject.Description, "DescriptiveText" );
      }

      [TestMethod]
      public void PropertiesWithoutEntryTranslatesToSelf()
      {
         Assert.AreEqual( "Id", _translator.TraslatedName( "Id" ) );
         Assert.AreEqual( "Id", _translator.TranslatedName( () => _testObject.Id ) );

         Assert.AreEqual( "Name", _translator.TraslatedName( "Name" ) );
         Assert.AreEqual( "Name", _translator.TranslatedName( () => _testObject.Name ) );
      }

      [TestMethod]
      public void PropertiesWithEntriesTranslatesToEntries()
      {
         Assert.AreEqual( "Profit", _translator.TraslatedName( "GrossMargin" ) );
         Assert.AreEqual( "Profit", _translator.TranslatedName( () => _testObject.GrossMargin ) );

         Assert.AreEqual( "DescriptiveText", _translator.TraslatedName( "Description" ) );
         Assert.AreEqual( "DescriptiveText", _translator.TranslatedName( () => _testObject.Description ) );
      }
   }
}
