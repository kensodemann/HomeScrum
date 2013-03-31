using System;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class SprintStatusTest
   {
      [TestMethod]
      public void IsOpenDisplayName()
      {
         var model = new SprintStatus();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsOpenStatus );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "SprintStatusIsOpenStatus", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.SprintStatusIsOpenStatus ) );
      }

      [TestMethod]
      public void CopyConstructor_CopiesAllProperties()
      {
         var model = new SprintStatus()
         {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            IsPredefined = true,
            AllowUse = true,
            IsOpenStatus = true
         };

         var newModel = new SprintStatus( model );

         Assert.AreNotSame( model, newModel );
         Assert.AreEqual( model.Id, newModel.Id );
         Assert.AreEqual( model.Name, newModel.Name );
         Assert.AreEqual( model.Description, newModel.Description );
         Assert.AreEqual( model.StatusCd, newModel.StatusCd );
         Assert.AreEqual( model.IsPredefined, newModel.IsPredefined );
         Assert.AreEqual( model.IsOpenStatus, newModel.IsOpenStatus );
      }
   }
}
