using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Common.TestData;
using NHibernate;
using Moq;

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
         _session = Database.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );

         Database.Build(_session);
         Users.Load(_sessionFactory.Object);
         ProjectStatuses.Load(_sessionFactory.Object);
         Projects.Load(_sessionFactory.Object);
      }

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      [TestMethod]
      public void IsNotValid_IfNoProjectStatusAssigned()
      {
         var project = new Project( _sessionFactory.Object )
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
         var project = new Project( _sessionFactory.Object )
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
