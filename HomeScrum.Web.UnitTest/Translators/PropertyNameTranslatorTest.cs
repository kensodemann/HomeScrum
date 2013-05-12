using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Translators;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.UnitTest.Translators
{
   [TestClass]
   public class PropertyNameTranslatorTest
   {
      class ChildClass
      {
         public Guid Id { get; set; }
         public string Name { get; set; }
         public string Description { get; set; }
      }
      
      class SourceTestClass
      {
         public Guid Id { get; set; }
         public string Name { get; set; }
         public string Description { get; set; }
         public Decimal GrossMargin { get; set; }
         public ChildClass Child { get; set; }
      }

      class TargetTestClass
      {
         public Guid Id { get; set; }
         public string Name { get; set; }
         public string DescriptiveText { get; set; }
         public Decimal Profit { get; set; }
         public Guid ChildId{get;set;}
         public string ChildName{get;set;}
      }

      private PropertyNameTranslator<SourceTestClass, TargetTestClass> _translator;
      private SourceTestClass _testObject;

      [TestInitialize]
      public void InitializeTest()
      {
         BuildTestObjects();
         SetupTranslator();
      }

      private void BuildTestObjects()
      {
         _translator = new PropertyNameTranslator<SourceTestClass, TargetTestClass>();
         _testObject = new SourceTestClass();
      }

      private void SetupTranslator()
      {
         _translator.AddTranslation( "GrossMargin", "Profit" );
         _translator.AddTranslation( () => _testObject.Description, "DescriptiveText" );
      }

      [TestMethod]
      public void MatchingPropertiesMap()
      {
         Assert.AreEqual( "Id", _translator.TranslatedName( "Id" ) );
         Assert.AreEqual( "Id", _translator.TranslatedName( () => _testObject.Id ) );

         Assert.AreEqual( "Name", _translator.TranslatedName( "Name" ) );
         Assert.AreEqual( "Name", _translator.TranslatedName( () => _testObject.Name ) );
      }

      [TestMethod]
      public void PropertiesWithEntriesTranslatesToEntries()
      {
         Assert.AreEqual( "Profit", _translator.TranslatedName( "GrossMargin" ) );
         Assert.AreEqual( "Profit", _translator.TranslatedName( () => _testObject.GrossMargin ) );

         Assert.AreEqual( "DescriptiveText", _translator.TranslatedName( "Description" ) );
         Assert.AreEqual( "DescriptiveText", _translator.TranslatedName( () => _testObject.Description ) );
      }
   }
}
