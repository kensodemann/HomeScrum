using System;
using System.Collections.Generic;
using System.Linq;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Extensions.Logging;

namespace HomeScrum.Data.UnitTest.Repositories
{
   [TestClass]
   public class WorkItemTypeRepositoryTest
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
         WorkItemTypes.Load();
         _logger = new Mock<ILogger>();
         _repository = new SimpleSortedRepository<WorkItemType>( _logger.Object );
      }

      private IRepository<WorkItemType> _repository;
      private Mock<ILogger> _logger;

      [TestMethod]
      public void GetAll_ReturnsAllWorkItemTypes()
      {
         var workItemTypes = _repository.GetAll();

         Assert.AreEqual( WorkItemTypes.ModelData.GetLength( 0 ), workItemTypes.Count );
         foreach (var wit in WorkItemTypes.ModelData)
         {
            AssertCollectionContainsWorkItemType( workItemTypes, wit );
         }
      }

      [TestMethod]
      public void GetAll_ReturnsItemsInSortOrder()
      {
         SwapSortOrders( WorkItemTypes.ModelData[0].Id, WorkItemTypes.ModelData[1].Id );
         var workItemTypes = _repository.GetAll();

         int previousSortSequence = 0;
         foreach (var item in workItemTypes)
         {
            Assert.IsTrue( item.SortSequence > previousSortSequence,
                String.Format( "List out of order.  Current: {0}, Previouis {1}", item.SortSequence, previousSortSequence ) );
            previousSortSequence = item.SortSequence;
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
         var workItemType = _repository.Get( WorkItemTypes.ModelData[2].Id );

         AssertWorkItemTypesAreEqual( WorkItemTypes.ModelData[2], workItemType );
      }


      [TestMethod]
      public void Add_AddsWorkItemTypeToDatabase()
      {
         var workItemType = new WorkItemType()
         {
            Name = "New WorkItem Type",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsTask = true,
            IsPredefined = true
         };

         _repository.Add( workItemType );
         Assert.AreEqual( WorkItemTypes.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsWorkItemType( _repository.GetAll(), workItemType );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var workItemType = WorkItemTypes.ModelData[3];

         workItemType.Name += "Modified";

         _repository.Update( workItemType );

         Assert.AreEqual( WorkItemTypes.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertWorkItemTypesAreEqual( workItemType, _repository.Get( workItemType.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var workItemType = WorkItemTypes.ModelData[2];

         _repository.Delete( workItemType );

         Assert.AreEqual( WorkItemTypes.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == workItemType.Id ) );
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

      private void SwapSortOrders( Guid witOneId, Guid witTwoId )
      {
         var item1 = _repository.Get( witOneId );
         var item2 = _repository.Get( witTwoId );
         int temp = item1.SortSequence;
         item1.SortSequence = item2.SortSequence;
         item2.SortSequence = temp;

         _repository.Update( item1 );
         _repository.Update( item2 );
      }
   }
}
