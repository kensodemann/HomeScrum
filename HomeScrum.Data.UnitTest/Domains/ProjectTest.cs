using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectTest
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

         var model = new Project()
         {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            LastModifiedUserRid = Guid.NewGuid(),
            ProjectStatus = projectStatus
         };

         var newModel = new Project( model );

         Assert.AreNotSame( model, newModel );
         Assert.AreEqual( model.Id, newModel.Id );
         Assert.AreEqual( model.Name, newModel.Name );
         Assert.AreEqual( model.Description, newModel.Description );
         Assert.AreEqual( model.LastModifiedUserRid, newModel.LastModifiedUserRid );
         Assert.AreNotSame( model.ProjectStatus, newModel.ProjectStatus );
         Assert.AreEqual( model.ProjectStatus.Id, newModel.ProjectStatus.Id );
         Assert.AreEqual( model.ProjectStatus.Name, newModel.ProjectStatus.Name );
      }
   }
}
