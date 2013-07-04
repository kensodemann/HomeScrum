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
   public class ProjectRepositoryTest
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
         ProjectStatuses.Load();
         Projects.Load();
         _logger = new Mock<ILogger>();
         _repository = new ProjectRepository( _logger.Object );
      }

      private IRepository<Project> _repository;
      private Mock<ILogger> _logger;

      [TestMethod]
      public void GetAll_ReturnsAllProjects()
      {
         var projects = _repository.GetAll();

         Assert.AreEqual( Projects.ModelData.GetLength( 0 ), projects.Count );
         foreach (var project in Projects.ModelData)
         {
            AssertCollectionContainsProject( projects, project );
         }
      }

      [TestMethod]
      public void GetAll_ReturnsProjetsInStatusOrder()
      {
         var projects = _repository.GetAll();

         int previousSortSequence = 0;
         foreach (var project in projects)
         {
            Assert.IsTrue( project.Status.SortSequence >= previousSortSequence,
                String.Format( "List out of order.  Current: {0}, Previouis {1}", project.Status.SortSequence, previousSortSequence ) );
            previousSortSequence = project.Status.SortSequence;
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
            Status = ProjectStatuses.ModelData[1],
            LastModifiedUserRid = Users.ModelData[0].Id
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
      public void Update_ModifiesProjectStatus()
      {
         var activeStatus = ProjectStatuses.ModelData.First( x => x.Name == "Open" );
         var inactiveStatus = ProjectStatuses.ModelData.First( x => x.Name == "Inactive" );
         var project = Projects.ModelData.First( x => x.Status.Id == activeStatus.Id );

         project.Status = new ProjectStatus() { Id = inactiveStatus.Id };
         _repository.Update( project );
         project = _repository.Get( project.Id );

         Assert.AreEqual( "Inactive", project.Status.Name );
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
         Assert.AreEqual( expected.LastModifiedUserRid, actual.LastModifiedUserRid );
         AssertProjectStatus( expected, actual );
      }

      private static void AssertProjectStatus( Project expected, Project actual )
      {
         Assert.AreNotSame( expected.Status, actual.Status );
         Assert.AreEqual( expected.Status.Id, actual.Status.Id );
         Assert.AreEqual( expected.Status.Name, actual.Status.Name );
         Assert.AreEqual( expected.Status.Description, actual.Status.Description );
      }
   }
}
