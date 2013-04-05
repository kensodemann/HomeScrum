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
   public class ProjectStatusRepositoryTest
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
         ProjectStatuses.Load();
         _repository = new Repository<ProjectStatus>();
      }

      private IRepository<ProjectStatus> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllProjectStatuses()
      {
         var statuses = _repository.GetAll();

         Assert.AreEqual( ProjectStatuses.ModelData.GetLength( 0 ), statuses.Count );
         foreach (var status in ProjectStatuses.ModelData)
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
         var status = _repository.Get( ProjectStatuses.ModelData[2].Id );

         AssertStatusesAreEqual( ProjectStatuses.ModelData[2], status );
      }


      [TestMethod]
      public void Add_AddsProjectStatusToDatabase()
      {
         var status = new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New one for Insert",
            AllowUse = true,
            IsActive = true,
            IsPredefined = true
         };

         _repository.Add( status );
         Assert.AreEqual( ProjectStatuses.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsStatus( _repository.GetAll(), status );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var status = ProjectStatuses.ModelData[3];

         status.Name += "Modified";

         _repository.Update( status );

         Assert.AreEqual( ProjectStatuses.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertStatusesAreEqual( status, _repository.Get( status.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var status = ProjectStatuses.ModelData[2];

         _repository.Delete( status );

         Assert.AreEqual( ProjectStatuses.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
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
         Assert.AreEqual( expected.AllowUse, actual.AllowUse );
         Assert.AreEqual( expected.IsActive, actual.IsActive );
         Assert.AreEqual( expected.IsPredefined, actual.IsPredefined );
      }
   }
}
