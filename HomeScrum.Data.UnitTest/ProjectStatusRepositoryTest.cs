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
   public class ProjectStatusRepositoryTest
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
         _repository = new DataObjectRepository<ProjectStatus>();
      }

      private IDataObjectRepository<ProjectStatus> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllProjectStatuses()
      {
         var statuses = _repository.GetAll();

         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ), statuses.Count );
         foreach (var status in TestData.ProjectStatuses)
         {
            AssertCollectionContainsStatus( statuses, status );
         }
      }

      [TestMethod]
      public void GetNonExistentProjectStatus_ReturnsNull()
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
      public void Get_ReturnsProjectStatus()
      {
         var status = _repository.Get( TestData.ProjectStatuses[2].Id );

         AssertStatusesAreEqual( TestData.ProjectStatuses[2], status );
      }


      [TestMethod]
      public void Add_AddsProjectStatusToDatabase()
      {
         var status = new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsActive = 'Y',
            IsPredefined = 'Y'
         };

         _repository.Add( status );
         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsStatus( _repository.GetAll(), status );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var status = TestData.ProjectStatuses[3];

         status.Name += "Modified";

         _repository.Update( status );

         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ), _repository.GetAll().Count );
         AssertStatusesAreEqual( status, _repository.Get( status.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var status = TestData.ProjectStatuses[2];

         _repository.Delete( status );

         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == status.Id ) );
      }


      private void AssertCollectionContainsStatus( ICollection<ProjectStatus> statuses, ProjectStatus status )
      {
         var statusFromCollection = statuses.FirstOrDefault( x => x.Id == status.Id );

         Assert.IsNotNull( statusFromCollection );
         AssertStatusesAreEqual( status, statusFromCollection );
      }

      private static void AssertStatusesAreEqual( ProjectStatus expected, ProjectStatus actual )
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
