using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Translators;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.UnitTest.Translators
{
   [TestClass]
   public class WorkItemPropertyNameTranslatorTest
   {
      private WorkItemEditorViewModelPropertyNameTranslator _translator;
      private WorkItem _sourceObject;

      [TestInitialize]
      public void InitializeTest()
      {
         _translator = new WorkItemEditorViewModelPropertyNameTranslator();
         _sourceObject = new WorkItem();
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
      public void AssignedToUserTranslatesToAssignedToUserId()
      {
         Assert.AreEqual( "AssignedToUserId", _translator.TranslatedName( "AssignedToUser" ) );
         Assert.AreEqual( "AssignedToUserId", _translator.TranslatedName( () => _sourceObject.AssignedToUser ) );
      }

      [TestMethod]
      public void CreatedByUserTranslatesToCreatedByUserName()
      {
         Assert.AreEqual( "CreatedByUserId", _translator.TranslatedName( "CreatedByUser" ) );
         Assert.AreEqual( "CreatedByUserId", _translator.TranslatedName( () => _sourceObject.CreatedByUser ) );
      }

      [TestMethod]
      public void LastModifiedUserRidTranslatesToLastModifiedUserId()
      {
         Assert.AreEqual( "LastModifiedUserId", _translator.TranslatedName( "LastModifiedUserRid" ) );
         Assert.AreEqual( "LastModifiedUserId", _translator.TranslatedName( () => _sourceObject.LastModifiedUserRid ) );
      }

      //[TestMethod]
      //public void ParentWorkItemTranslatesToParentWorkItemId()
      //{
      //   Assert.AreEqual( "ParentWorkItemId", _translator.TranslatedName( "ParentWorkItem" ) );
      //   Assert.AreEqual( "ParentWorkItemId", _translator.TranslatedName( () => _sourceObject.ParentWorkItem ) );
      //}

      [TestMethod]
      public void ProjectTranslatesToProjectId()
      {
         Assert.AreEqual( "ProjectId", _translator.TranslatedName( "Project" ) );
         Assert.AreEqual( "ProjectId", _translator.TranslatedName( () => _sourceObject.Project ) );
      }

      [TestMethod]
      public void StatusTranslatesToStatusId()
      {
         Assert.AreEqual( "StatusId", _translator.TranslatedName( "Status" ) );
         Assert.AreEqual( "StatusId", _translator.TranslatedName( () => _sourceObject.Status ) );
      }

      [TestMethod]
      public void WorkItemStatusTranslatesToWorkItemStatusId()
      {
         Assert.AreEqual( "WorkItemTypeId", _translator.TranslatedName( "WorkItemType" ) );
         Assert.AreEqual( "WorkItemTypeId", _translator.TranslatedName( () => _sourceObject.WorkItemType ) );
      }
   }
}
