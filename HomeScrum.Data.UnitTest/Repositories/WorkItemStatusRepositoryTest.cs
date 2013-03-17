using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeScrum.Data.UnitTest.Repositories
{
   [TestClass]
   public class WorkItemStatusRepositoryTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext context )
      {
         Database.Initialize();
      }


      [TestInitialize]
      public void InitializeTest()
      {
         Database.Build();
         WorkItemStatuses.Load();
         _repository = new Repository<WorkItemStatus>();
      }

      private IRepository<WorkItemStatus> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllWorkItemStatuses()
      {
         var statuses = _repository.GetAll();

         Assert.AreEqual( WorkItemStatuses.ModelData.GetLength( 0 ), statuses.Count );
         foreach (var status in WorkItemStatuses.ModelData)
         {
            AssertCollectionContainsStatus( statuses, status );
         }
      }

      [TestMethod]
      public void GetNonExistentWorkItemStatus_ReturnsNull()
      {
         var status = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( status );
      }

      [TestMethod]
      public void GetUsingDefaultGuid_ReturnsNull()
      {
         var status = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( status );
      }

      [TestMethod]
      public void Get_ReturnsWorkItemStatus()
      {
         var status = _repository.Get( WorkItemStatuses.ModelData[2].Id );

         AssertStatusesAreEqual( WorkItemStatuses.ModelData[2], status );
      }


      [TestMethod]
      public void Add_AddsWorkItemStatusToDatabase()
      {
         var status = new WorkItemStatus()
         {
            Name = "New WorkItem Status",
            Description = "New one for Insert",
            AllowUse = true,
            IsOpenStatus = true,
            IsPredefined = true
         };

         _repository.Add( status );
         Assert.AreEqual( WorkItemStatuses.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsStatus( _repository.GetAll(), status );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var status = WorkItemStatuses.ModelData[3];

         status.Name += "Modified";

         _repository.Update( status );

         Assert.AreEqual( WorkItemStatuses.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertStatusesAreEqual( status, _repository.Get( status.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var status = WorkItemStatuses.ModelData[2];

         _repository.Delete( status );

         Assert.AreEqual( WorkItemStatuses.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == status.Id ) );
      }


      private void AssertCollectionContainsStatus( ICollection<WorkItemStatus> statuses, WorkItemStatus status )
      {
         var statusFromCollection = statuses.FirstOrDefault( x => x.Id == status.Id );

         Assert.IsNotNull( statusFromCollection );
         AssertStatusesAreEqual( status, statusFromCollection );
      }

      private static void AssertStatusesAreEqual( WorkItemStatus expected, WorkItemStatus actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.AllowUse, actual.AllowUse );
         Assert.AreEqual( expected.IsOpenStatus, actual.IsOpenStatus );
         Assert.AreEqual( expected.IsPredefined, actual.IsPredefined );
      }
   }
}
