using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Translators;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.UnitTest.Translators
{
   [TestClass]
   public class ProjectPropertyNameTranslatorTest
   {
      private ProjectPropertyNameTranslator _translator;
      private Project _sourceObject;

      [TestInitialize]
      public void InitializeTest()
      {
         _translator = new ProjectPropertyNameTranslator();
         _sourceObject = new Project();
      }

      [TestMethod]
      public void IdTranslatesToId()
      {
         Assert.AreEqual( "Id", _translator.TranslatedName( "Id" ) );
         Assert.AreEqual( "Id", _translator.TranslatedName( () => _sourceObject.Id ) );
      }

      [TestMethod]
      public void NameTranslatedToName()
      {
         Assert.AreEqual( "Name", _translator.TranslatedName( "Name" ) );
         Assert.AreEqual( "Name", _translator.TranslatedName( () => _sourceObject.Name ) );
      }

      [TestMethod]
      public void DescriptionTranslesToDescription()
      {
         Assert.AreEqual( "Description", _translator.TranslatedName( "Description" ) );
         Assert.AreEqual( "Description", _translator.TranslatedName( () => _sourceObject.Description ) );
      }


      [TestMethod]
      public void LastModifiedUserRidTranslatesToLastModifiedUserId()
      {
         Assert.AreEqual( "LastModifiedUserId", _translator.TranslatedName( "LastModifiedUserRid" ) );
         Assert.AreEqual( "LastModifiedUserId", _translator.TranslatedName( () => _sourceObject.LastModifiedUserRid ) );
      }

      [TestMethod]
      public void StatusTranslatesToStatusId()
      {
         Assert.AreEqual( "StatusId", _translator.TranslatedName( "Status" ) );
         Assert.AreEqual( "StatusId", _translator.TranslatedName( () => _sourceObject.Status ) );
      }
   }
}
