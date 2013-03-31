using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class WorkItemTypeTest
   {
      [TestMethod]
      public void IsTaskDisplayName()
      {
         var model = new WorkItemType();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsTask );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.WorkItemTypeIsTask ) );
         Assert.AreEqual( "WorkItemTypeIsTask", display.Name );
      }

      [TestMethod]
      public void CopyConstructor_CopiesAllProperties()
      {
         var model = new WorkItemType()
         {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            IsPredefined = true,
            AllowUse = true,
            IsTask = true
         };

         var newModel = new WorkItemType( model );

         Assert.AreNotSame( model, newModel );
         Assert.AreEqual( model.Id, newModel.Id );
         Assert.AreEqual( model.Name, newModel.Name );
         Assert.AreEqual( model.Description, newModel.Description );
         Assert.AreEqual( model.StatusCd, newModel.StatusCd );
         Assert.AreEqual( model.IsPredefined, newModel.IsPredefined );
         Assert.AreEqual( model.IsTask, newModel.IsTask );
      }
   }
}
