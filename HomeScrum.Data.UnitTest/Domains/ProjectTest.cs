using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Common.TestData;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext ctx )
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
      }

      [TestMethod]
      public void IsNotValid_IfNoProjectStatusAssigned()
      {
         var project = new Project()
         {
            Id = Guid.NewGuid(),
            Name = "New Project",
            Description = "This is just for testing"
         };

         Assert.IsFalse( project.IsValidFor( TransactionType.All ) );
         var messages = project.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( "The Status field is required.", messages["Status"] );
      }

      [TestMethod]
      public void IsNotValid_IfDifferentProjectWithSameNameExists()
      {
         var project = new Project()
         {
            Id = Guid.NewGuid(),
            Name = Projects.ModelData[0].Name,
            Description = "This is just for testing",
            Status = ProjectStatuses.ModelData[0]
         };

         Assert.IsFalse( project.IsValidFor( TransactionType.All ) );
         var messages = project.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Project", project.Name ), messages["Name"] );
      }

      [TestMethod]
      public void IsValid_IfExistingValidProject()
      {
         var project = Projects.ModelData[0];

         Assert.IsTrue( project.IsValidFor( TransactionType.All ) );
         var messages = project.GetErrorMessages();
         Assert.AreEqual( 0, messages.Count );
      }
   }
}
