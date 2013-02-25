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
         var ProjectStatus = _repository.GetAll();

         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ), ProjectStatus.Count );
         foreach (var wit in TestData.ProjectStatuses)
         {
            AssertCollectionContainsProjectStatus( ProjectStatus, wit );
         }
      }

      [TestMethod]
      public void GetNonExistentProjectStatus_ReturnsNull()
      {
         var projectStatus = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( projectStatus );
      }

      [TestMethod]
      public void GetUsingDefaultGuid_ReturnsNull()
      {
         var projectStatus = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( projectStatus );
      }

      [TestMethod]
      public void Get_ReturnsProjectStatus()
      {
         var projectStatus = _repository.Get( TestData.ProjectStatuses[2].Id );

         AssertProjectStatusesAreEqual( TestData.ProjectStatuses[2], projectStatus );
      }


      [TestMethod]
      public void Add_AddsProjectStatusToDatabase()
      {
         var projectStatus = new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New one for Insert",
            StatusCd = 'A',
            IsActive = 'Y',
            IsPredefined = 'Y'
         };

         _repository.Add( projectStatus );
         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsProjectStatus( _repository.GetAll(), projectStatus );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var projectStatus = TestData.ProjectStatuses[3];

         projectStatus.Name += "Modified";

         _repository.Update( projectStatus );

         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ), _repository.GetAll().Count );
         AssertProjectStatusesAreEqual( projectStatus, _repository.Get( projectStatus.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var projectStatus = TestData.ProjectStatuses[2];

         _repository.Delete( projectStatus );

         Assert.AreEqual( TestData.ProjectStatuses.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == projectStatus.Id ) );
      }


      private void AssertCollectionContainsProjectStatus( ICollection<ProjectStatus> projectStatuses, ProjectStatus projectStatus )
      {
         var projectStatusFromCollection = projectStatuses.FirstOrDefault( x => x.Id == projectStatus.Id );

         Assert.IsNotNull( projectStatusFromCollection );
         AssertProjectStatusesAreEqual( projectStatus, projectStatusFromCollection );
      }

      private static void AssertProjectStatusesAreEqual( ProjectStatus expected, ProjectStatus actual )
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
