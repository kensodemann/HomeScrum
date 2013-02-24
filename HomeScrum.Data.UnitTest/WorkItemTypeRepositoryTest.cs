using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Domain;
using HomeScrum.Data.SqlServer;
using System.Collections.Generic;
using System.Linq;

namespace HomeScrum.Data.UnitTest
{
   [TestClass]
   public class WorkItemTypeRepositoryTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext context )
      {
         TestData.Initialize();
      }


      [TestInitialize]
      public void InitializeTest()
      {
         TestData.BuildDatabase();
         _repository = new DataObjectRepository<WorkItemType>();
      }

      private IDataObjectRepository<WorkItemType> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllWorkItemTypes()
      {
         var workItemTypes = _repository.GetAll();

         Assert.AreEqual( TestData.WorkItemTypes.GetLength( 0 ), workItemTypes.Count );
         foreach (var wit in TestData.WorkItemTypes)
         {
            AssertCollectionContainsWorkItemType( workItemTypes, wit );
         }
      }

      [TestMethod]
      public void GetNonExistentWorkItemType_ReturnsNull()
      {
         var workItemType = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( workItemType );
      }

      [TestMethod]
      public void GetUsingDefaultGuid_ReturnsNull()
      {
         var workItemType = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( workItemType );
      }

      [TestMethod]
      public void Get_ReturnsWorkItemType()
      {
         var workItemType = _repository.Get( TestData.WorkItemTypes[2].Id );

         AssertWorkItemTypesAreEqual( TestData.WorkItemTypes[2], workItemType );
      }


      private void AssertCollectionContainsWorkItemType( ICollection<WorkItemType> workItemTypes, WorkItemType workItemType )
      {
         var workItemTypeFromCollection = workItemTypes.FirstOrDefault( x => x.Id == workItemType.Id );

         Assert.IsNotNull( workItemTypeFromCollection );
         AssertWorkItemTypesAreEqual( workItemType, workItemTypeFromCollection );
      }

      private static void AssertWorkItemTypesAreEqual( WorkItemType expected, WorkItemType actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.StatusCd, actual.StatusCd );
         Assert.AreEqual( expected.IsTask, actual.IsTask );
         Assert.AreEqual( expected.IsPredefined, actual.IsPredefined );
      }
   }
}
