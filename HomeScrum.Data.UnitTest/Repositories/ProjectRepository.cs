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
   public class ProjectRepository
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
         Projects.Load();
         _repository = new Repository<Project, Guid>();
      }

      private IRepository<Project, Guid> _repository;

      [TestMethod]
      public void GetAll_ReturnsAllProjects()
      {
         var projects = _repository.GetAll();

         Assert.AreEqual( Projects.ModelData.GetLength( 0 ), projects.Count );
         foreach (var status in Projects.ModelData)
         {
            AssertCollectionContainsProject( projects, status );
         }
      }

      [TestMethod]
      public void GetNonExistentProject_ReturnsNull()
      {
         var project = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( project );
      }

      [TestMethod]
      public void GetUsingDefaultGuid_ReturnsNull()
      {
         var project = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( project );
      }

      [TestMethod]
      public void Get_ReturnsProject()
      {
         var project = _repository.Get( Projects.ModelData[2].Id );

         AssertProjectsAreEqual( Projects.ModelData[2], project );
      }


      [TestMethod]
      public void Add_AddsProjectToDatabase()
      {
         var project = new Project()
         {
            Name = "New Project",
            Description = "New one for Insert",
            ProjectStatusRid = ProjectStatuses.ModelData[1].Id,
            LastModifiedUserId = "Frank"
         };

         _repository.Add( project );
         Assert.AreEqual( Projects.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsProject( _repository.GetAll(), project );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var project = Projects.ModelData[3];

         project.Name += "Modified";

         _repository.Update( project );

         Assert.AreEqual( Projects.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertProjectsAreEqual( project, _repository.Get( project.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var project = Projects.ModelData[2];

         _repository.Delete( project );

         Assert.AreEqual( Projects.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.Id == project.Id ) );
      }

      private void AssertCollectionContainsProject( ICollection<Project> projects, Project project )
      {
         var statusFromCollection = projects.FirstOrDefault( x => x.Id == project.Id );

         Assert.IsNotNull( statusFromCollection );
         AssertProjectsAreEqual( project, statusFromCollection );
      }

      private static void AssertProjectsAreEqual( Project expected, Project actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.ProjectStatusRid, actual.ProjectStatusRid );
         Assert.AreEqual( expected.LastModifiedUserId, actual.LastModifiedUserId );
      }
   }
}
