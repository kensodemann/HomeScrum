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
   public class SprintStatusRepositoryTest
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
         _repository = new DataObjectRepository<SprintStatus>();
      }

      private IDataObjectRepository<SprintStatus> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllSprintStatuses()
      {
         var sprintStatuses = _repository.GetAll();

         Assert.AreEqual( TestData.SprintStatuses.GetLength( 0 ), sprintStatuses.Count );
         foreach (var wit in TestData.SprintStatuses)
         {
            AssertCollectionContainsSprintStatus( sprintStatuses, wit );
         }
      }

      [TestMethod]
      public void GetNonExistentSprintStatus_ReturnsNull()
      {
         var sprintStatus = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( sprintStatus );
      }

      [TestMethod]
      public void GetUsingDefaultGuid_ReturnsNull()
      {
         var sprintStatus = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( sprintStatus );
      }

      [TestMethod]
      public void Get_ReturnsSprintStatus()
      {
         var sprintStatus = _repository.Get( TestData.SprintStatuses[2].Id );

         AssertSprintStatusesAreEqual( TestData.SprintStatuses[2], sprintStatus );
      }


      [TestMethod]
      public void Add_AddsSprintStatusToDatabase()
      {
         var sprintStatus = new SprintStatus()
         {
            Name = "New Sprint Type",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsOpenStatus = 'Y',
            IsPredefined = 'Y'
         };

         _repository.Add( sprintStatus );
         Assert.AreEqual( TestData.SprintStatuses.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsSprintStatus( _repository.GetAll(), sprintStatus );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var sprintStatus = TestData.SprintStatuses[3];

         sprintStatus.Name += "Modified";

         _repository.Update( sprintStatus );

         Assert.AreEqual( TestData.SprintStatuses.GetLength( 0 ), _repository.GetAll().Count );
         AssertSprintStatusesAreEqual( sprintStatus, _repository.Get( sprintStatus.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var sprintStatus = TestData.SprintStatuses[2];

         _repository.Delete( sprintStatus );

         Assert.AreEqual( TestData.SprintStatuses.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == sprintStatus.Id ) );
      }


      private void AssertCollectionContainsSprintStatus( ICollection<SprintStatus> sprintStatuses, SprintStatus sprintStatus )
      {
         var sprintStatusFromCollection = sprintStatuses.FirstOrDefault( x => x.Id == sprintStatus.Id );

         Assert.IsNotNull( sprintStatusFromCollection );
         AssertSprintStatusesAreEqual( sprintStatus, sprintStatusFromCollection );
      }

      private static void AssertSprintStatusesAreEqual( SprintStatus expected, SprintStatus actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.StatusCd, actual.StatusCd );
         Assert.AreEqual( expected.IsOpenStatus, actual.IsOpenStatus );
         Assert.AreEqual( expected.IsPredefined, actual.IsPredefined );
      }
   }
}
