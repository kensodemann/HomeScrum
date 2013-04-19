using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.SqlServer;
using HomeScrum.Data.Repositories;
using System.Collections.Generic;

namespace HomeScrum.Data.UnitTest.Repositories
{
   [TestClass]
   public class WorkItemRepositoryTest
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
         Users.Load();
         WorkItemStatuses.Load();
         WorkItemTypes.Load();
         ProjectStatuses.Load();
         Projects.Load();
         SprintStatuses.Load();
         //Sprints.Load();
         AcceptanceCriteriaStatuses.Load();
         WorkItems.Load();
         _repository = new Repository<WorkItem>();
      }

      private IRepository<WorkItem> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllWorkItems()
      {
         var workItems = _repository.GetAll();

         Assert.AreEqual( WorkItems.ModelData.GetLength( 0 ), workItems.Count );
         foreach (var workItem in WorkItems.ModelData)
         {
            AssertCollectionContainsWorkItem( workItems, workItem );
         }
      }

      [TestMethod]
      public void GetNonExistentWorkItem_ReturnsNull()
      {
         var workItem = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( workItem );
      }

      [TestMethod]
      public void GetUsingDefaultGuid_ReturnsNull()
      {
         var workItem = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( workItem );
      }

      [TestMethod]
      public void Get_ReturnsWorkItem()
      {
         var workItem = _repository.Get( WorkItems.ModelData[2].Id );

         AssertWorkItemsAreEqual( WorkItems.ModelData[2], workItem );
      }


      [TestMethod]
      public void Add_AddsWorkItemToDatabase()
      {
         var workItem = new WorkItem()
         {
            Name = "New Work Item",
            Description = "New one for Insert",
            Status = WorkItemStatuses.ModelData[1],
            Project = Projects.ModelData[0],
            WorkItemType = WorkItemTypes.ModelData[1],
            CreatedByUserRid = Users.ModelData[0].Id,
            LastModifiedUserRid = Users.ModelData[0].Id
         };
         workItem.AcceptanceCriteria = new[]
         {
            new AcceptanceCriteria()
            {
               Name = "It is accepted",
               Description = "It must be accepted in order to be accepted.",
               Status = AcceptanceCriteriaStatuses.ModelData[0],
               WorkItem = workItem
            }
         };

         _repository.Add( workItem );
         Assert.AreEqual( WorkItems.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsWorkItem( _repository.GetAll(), workItem );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var workItem = WorkItems.ModelData[3];

         workItem.Name += "Modified";

         _repository.Update( workItem );

         Assert.AreEqual( WorkItems.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertWorkItemsAreEqual( workItem, _repository.Get( workItem.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var workItem = WorkItems.ModelData[2];

         _repository.Delete( workItem );

         Assert.AreEqual( WorkItems.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == workItem.Id ) );
      }


      private void AssertCollectionContainsWorkItem( ICollection<WorkItem> workItems, WorkItem workItem )
      {
         var itemFromCollection = workItems.FirstOrDefault( x => x.Id == workItem.Id );

         Assert.IsNotNull( itemFromCollection );
         AssertWorkItemsAreEqual( workItem, itemFromCollection );
      }

      private static void AssertWorkItemsAreEqual( WorkItem expected, WorkItem actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.AssignedToUserRid, actual.AssignedToUserRid );
         Assert.AreEqual( expected.CreatedByUserRid, actual.CreatedByUserRid );
         Assert.AreEqual( expected.LastModifiedUserRid, actual.LastModifiedUserRid );
         Assert.AreEqual( expected.Status.Id, actual.Status.Id );
         Assert.AreEqual( expected.WorkItemType.Id, actual.WorkItemType.Id );
         Assert.AreEqual( expected.Project.Id, actual.Project.Id );

         if (expected.ParentWorkItem == null)
         {
            Assert.IsTrue( actual.ParentWorkItem == null || actual.ParentWorkItem.Id == default( Guid ) );
         }
         else
         {
            Assert.AreEqual( expected.ParentWorkItem.Id, actual.ParentWorkItem.Id );
         }

         if (expected.AcceptanceCriteria == null)
         {
            Assert.AreEqual( 0, actual.AcceptanceCriteria.Count() );
         }
         else
         {
            Assert.AreEqual( expected.AcceptanceCriteria.Count(), actual.AcceptanceCriteria.Count() );
         }
      }
   }
}
