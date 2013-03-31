using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectHistoryTest
   {
      [TestMethod]
      public void CopyConstructor_CopiesAllProperties()
      {
         var projectStatus = new ProjectStatus()
         {
            Id = Guid.NewGuid(),
            Name = "ProjectStatus",
            AllowUse = true,
            IsActive = true,
            IsPredefined = true
         };

         var model = new ProjectHistory()
         {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            LastModifiedUserRid = Guid.NewGuid(),
            ProjectStatus = projectStatus,
            HistoryTimestamp = DateTime.Now,
            ProjectRid = Guid.NewGuid(),
            SequenceNumber = 42
         };

         var newModel = new ProjectHistory( model );

         Assert.AreNotSame( model, newModel );
         Assert.AreEqual( model.Id, newModel.Id );
         Assert.AreEqual( model.Name, newModel.Name );
         Assert.AreEqual( model.Description, newModel.Description );
         Assert.AreEqual( model.LastModifiedUserRid, newModel.LastModifiedUserRid );
         Assert.AreEqual( model.ProjectRid, newModel.ProjectRid );
         Assert.AreEqual( model.HistoryTimestamp, newModel.HistoryTimestamp );
         Assert.AreEqual( model.SequenceNumber, newModel.SequenceNumber );
         Assert.AreNotSame( model.ProjectStatus, newModel.ProjectStatus );
         Assert.AreEqual( model.ProjectStatus.Id, newModel.ProjectStatus.Id );
         Assert.AreEqual( model.ProjectStatus.Name, newModel.ProjectStatus.Name );
      }
   }
}
