using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Translators;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.UnitTest.Translators
{
   [TestClass]
   public class WorkItemPropertyNameTranslatorTest
   {
      private PropertyNameTranslatorBase _translator;
      private WorkItem _workItem;

      [TestInitialize]
      public void InitializeTest()
      {
         _translator = new PropertyNameTranslatorBase();
         _workItem = new WorkItem();
      }

      [TestMethod]
      public void IdTranslatesToId()
      {
         Assert.AreEqual( "Id", _translator.ViewModelPropertyName( "Id" ) );
         Assert.AreEqual( "Id", _translator.ViewModelPropertyName( () => _workItem.Id ) );
      }

      [TestMethod]
      public void NameTranslatesToName()
      {
         Assert.AreEqual( "Name", _translator.ViewModelPropertyName( "Name" ) );
         Assert.AreEqual( "Name", _translator.ViewModelPropertyName( () => _workItem.Name ) );
      }

      [TestMethod]
      public void DescriptionTranslatesToDescription()
      {
         Assert.AreEqual( "Description", _translator.ViewModelPropertyName( "Description" ) );
         Assert.AreEqual( "Description", _translator.ViewModelPropertyName( () => _workItem.Description ) );
      }
   }
}
