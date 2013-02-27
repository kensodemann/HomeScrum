﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using System.Collections.Generic;
using System.Linq;
using HomeScrum.Data.SqlServer;
using HomeScrum.Data.Repositories;

namespace HomeScrum.Data.UnitTest
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
         _repository = new DataObjectRepository<AcceptanceCriteriaStatus>();
      }

      private IDataObjectRepository<AcceptanceCriteriaStatus> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllAcceptanceCriteriaStatuses()
      {
         var statuses = _repository.GetAll();

         Assert.AreEqual( Database.AcceptanceCriteriaStatuses.GetLength( 0 ), statuses.Count );
         foreach (var status in Database.AcceptanceCriteriaStatuses)
         {
            AssertCollectionContainsStatus( statuses, status );
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
         var status = _repository.Get( Database.AcceptanceCriteriaStatuses[2].Id );

         AssertStatusesAreEqual( Database.AcceptanceCriteriaStatuses[2], status );
      }


      [TestMethod]
      public void Add_AddsAcceptanceCriteriaStatusToDatabase()
      {
         var status = new AcceptanceCriteriaStatus()
         {
            Name = "New Acceptance Criteria Status",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsAccepted = 'Y',
            IsPredefined = 'Y'
         };

         _repository.Add( status );
         Assert.AreEqual( Database.AcceptanceCriteriaStatuses.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsStatus( _repository.GetAll(), status );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var status = Database.AcceptanceCriteriaStatuses[3];

         status.Name += "Modified";

         _repository.Update( status );

         Assert.AreEqual( Database.AcceptanceCriteriaStatuses.GetLength( 0 ), _repository.GetAll().Count );
         AssertStatusesAreEqual( status, _repository.Get( status.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var status = Database.AcceptanceCriteriaStatuses[2];

         _repository.Delete( status );

         Assert.AreEqual( Database.AcceptanceCriteriaStatuses.GetLength( 0 ) - 1, _repository.GetAll().Count );
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
   }
}
