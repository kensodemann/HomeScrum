using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectTest
   {
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
   }
}
