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
   public class AcceptanceCriteriaStatusRepositoryTest
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
         AcceptanceCriteriaStatuses.Load();
         _logger = new Mock<ILogger>();
         _repository = new SimpleSortedRepository<AcceptanceCriteriaStatus>( _logger.Object );
      }

      private IRepository<AcceptanceCriteriaStatus> _repository;
      private Mock<ILogger> _logger;

      [TestMethod]
      public void GetAll_ReturnsAllAcceptanceCriteriaStatuses()
      {
         var statuses = _repository.GetAll();

         Assert.AreEqual( AcceptanceCriteriaStatuses.ModelData.GetLength( 0 ), statuses.Count );
         foreach (var status in AcceptanceCriteriaStatuses.ModelData)
         {
            AssertCollectionContainsStatus( statuses, status );
         }
      }

      [TestMethod]
      public void GetAll_ReturnsItemsInSortOrder()
      {
         SwapSortOrders( AcceptanceCriteriaStatuses.ModelData[0].Id, AcceptanceCriteriaStatuses.ModelData[1].Id );
         var statuses = _repository.GetAll();

         int previousSortSequence = 0;
         foreach (var item in statuses)
         {
            Assert.IsTrue( item.SortSequence > previousSortSequence,
                String.Format( "List out of order.  Current: {0}, Previouis {1}", item.SortSequence, previousSortSequence ) );
            previousSortSequence = item.SortSequence;
         }
      }

      [TestMethod]
      public void GetNonExistentAcceptanceCriteriaStatus_ReturnsNull()
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
      public void Get_ReturnsAcceptanceCriteriaStatus()
      {
         var status = _repository.Get( AcceptanceCriteriaStatuses.ModelData[2].Id );

         AssertStatusesAreEqual( AcceptanceCriteriaStatuses.ModelData[2], status );
      }


      [TestMethod]
      public void Add_AddsAcceptanceCriteriaStatusToDatabase()
      {
         var status = new AcceptanceCriteriaStatus()
         {
            Name = "New Acceptance Criteria Status",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsAccepted = true,
            IsPredefined = true
         };

         _repository.Add( status );
         Assert.AreEqual( AcceptanceCriteriaStatuses.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsStatus( _repository.GetAll(), status );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var status = AcceptanceCriteriaStatuses.ModelData[3];

         status.Name += "Modified";

         _repository.Update( status );

         Assert.AreEqual( AcceptanceCriteriaStatuses.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertStatusesAreEqual( status, _repository.Get( status.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var status = AcceptanceCriteriaStatuses.ModelData[2];

         _repository.Delete( status );

         Assert.AreEqual( AcceptanceCriteriaStatuses.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == status.Id ) );
      }


      private void AssertCollectionContainsStatus( ICollection<AcceptanceCriteriaStatus> statuses, AcceptanceCriteriaStatus status )
      {
         var statusFromCollection = statuses.FirstOrDefault( x => x.Id == status.Id );

         Assert.IsNotNull( statusFromCollection );
         AssertStatusesAreEqual( status, statusFromCollection );
      }

      private static void AssertStatusesAreEqual( AcceptanceCriteriaStatus expected, AcceptanceCriteriaStatus actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.StatusCd, actual.StatusCd );
         Assert.AreEqual( expected.IsAccepted, actual.IsAccepted );
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
