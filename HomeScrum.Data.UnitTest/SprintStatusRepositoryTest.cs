using HomeScrum.Common.TestData;
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
         Database.Initialize();
      }


      [TestInitialize]
      public void InitializeTest()
      {
         Database.Build();
         SprintStatuses.Load();
         _repository = new DataObjectRepository<SprintStatus>();
      }

      private IDataObjectRepository<SprintStatus> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllSprintStatuses()
      {
         var statuses = _repository.GetAll();

         Assert.AreEqual( SprintStatuses.ModelData.GetLength( 0 ), statuses.Count );
         foreach (var status in SprintStatuses.ModelData)
         {
            AssertCollectionContainsStatus( statuses, status );
         }
      }

      [TestMethod]
      public void GetNonExistentSprintStatus_ReturnsNull()
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
      public void Get_ReturnsSprintStatus()
      {
         var status = _repository.Get( SprintStatuses.ModelData[2].Id );

         AssertStatusesAreEqual( SprintStatuses.ModelData[2], status );
      }


      [TestMethod]
      public void Add_AddsSprintStatusToDatabase()
      {
         var status = new SprintStatus()
         {
            Name = "New Sprint Type",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsOpenStatus = 'Y',
            IsPredefined = 'Y'
         };

         _repository.Add( status );
         Assert.AreEqual( SprintStatuses.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsStatus( _repository.GetAll(), status );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var status = SprintStatuses.ModelData[3];

         status.Name += "Modified";

         _repository.Update( status );

         Assert.AreEqual( SprintStatuses.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertStatusesAreEqual( status, _repository.Get( status.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var status = SprintStatuses.ModelData[2];

         _repository.Delete( status );

         Assert.AreEqual( SprintStatuses.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == status.Id ) );
      }


      private void AssertCollectionContainsStatus( ICollection<SprintStatus> statusues, SprintStatus status )
      {
         var statusFromCollection = statusues.FirstOrDefault( x => x.Id == status.Id );

         Assert.IsNotNull( statusFromCollection );
         AssertStatusesAreEqual( status, statusFromCollection );
      }

      private static void AssertStatusesAreEqual( SprintStatus expected, SprintStatus actual )
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
