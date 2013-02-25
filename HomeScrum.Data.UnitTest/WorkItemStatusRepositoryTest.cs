using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeScrum.Data.UnitTest
{
   [TestClass]
   public class WorkItemStatusRepositoryTest
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
         _repository = new DataObjectRepository<WorkItemStatus>();
      }

      private IDataObjectRepository<WorkItemStatus> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllWorkItemStatuses()
      {
         var workItemStatuses = _repository.GetAll();

         Assert.AreEqual( TestData.WorkItemStatuses.GetLength( 0 ), workItemStatuses.Count );
         foreach (var wit in TestData.WorkItemStatuses)
         {
            AssertCollectionContainsWorkItemStatus( workItemStatuses, wit );
         }
      }

      [TestMethod]
      public void GetNonExistentWorkItemStatus_ReturnsNull()
      {
         var workItemStatus = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( workItemStatus );
      }

      [TestMethod]
      public void GetUsingDefaultGuid_ReturnsNull()
      {
         var workItemStatus = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( workItemStatus );
      }

      [TestMethod]
      public void Get_ReturnsWorkItemStatus()
      {
         var workItemStatus = _repository.Get( TestData.WorkItemStatuses[2].Id );

         AssertWorkItemStatusesAreEqual( TestData.WorkItemStatuses[2], workItemStatus );
      }


      [TestMethod]
      public void Add_AddsWorkItemStatusToDatabase()
      {
         var workItemStatus = new WorkItemStatus()
         {
            Name = "New WorkItem Type",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsActive = 'Y',
            IsPredefined = 'Y'
         };

         _repository.Add( workItemStatus );
         Assert.AreEqual( TestData.WorkItemStatuses.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsWorkItemStatus( _repository.GetAll(), workItemStatus );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var workItemStatus = TestData.WorkItemStatuses[3];

         workItemStatus.Name += "Modified";

         _repository.Update( workItemStatus );

         Assert.AreEqual( TestData.WorkItemStatuses.GetLength( 0 ), _repository.GetAll().Count );
         AssertWorkItemStatusesAreEqual( workItemStatus, _repository.Get( workItemStatus.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var workItemStatus = TestData.WorkItemStatuses[2];

         _repository.Delete( workItemStatus );

         Assert.AreEqual( TestData.WorkItemStatuses.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == workItemStatus.Id ) );
      }


      private void AssertCollectionContainsWorkItemStatus( ICollection<WorkItemStatus> workItemStatuses, WorkItemStatus workItemStatus )
      {
         var workItemStatusFromCollection = workItemStatuses.FirstOrDefault( x => x.Id == workItemStatus.Id );

         Assert.IsNotNull( workItemStatusFromCollection );
         AssertWorkItemStatusesAreEqual( workItemStatus, workItemStatusFromCollection );
      }

      private static void AssertWorkItemStatusesAreEqual( WorkItemStatus expected, WorkItemStatus actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.StatusCd, actual.StatusCd );
         Assert.AreEqual( expected.IsActive, actual.IsActive );
         Assert.AreEqual( expected.IsPredefined, actual.IsPredefined );
      }
   }
}
