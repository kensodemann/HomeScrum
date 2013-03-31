using System;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectStatusTest
   {
      [TestMethod]
      public void IsActiveDisplayName()
      {
         var model = new ProjectStatus();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsActive );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "ProjectStatusIsActive", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.ProjectStatusIsActive ) );
      }

      [TestMethod]
      public void CopyConstructor_CopiesAllProperties()
      {
         var model = new ProjectStatus()
         {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            IsPredefined = true,
            AllowUse = true,
            IsActive = true
         };

         var newModel = new ProjectStatus( model );

         Assert.AreNotSame( model, newModel );
         Assert.AreEqual( model.Id, newModel.Id );
         Assert.AreEqual( model.Name, newModel.Name );
         Assert.AreEqual( model.Description, newModel.Description );
         Assert.AreEqual( model.StatusCd, newModel.StatusCd );
         Assert.AreEqual( model.IsPredefined, newModel.IsPredefined );
         Assert.AreEqual( model.IsActive, newModel.IsActive );
      }
   }
}
