using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class SystemDataObjectTest
   {
      [TestMethod]
      public void StatusCdIsI_IfNotAllowUse()
      {
         var model = new SystemDataObject();

         model.AllowUse = false;

         Assert.AreEqual( 'I', model.StatusCd );
         Assert.IsFalse( model.AllowUse );
      }

      [TestMethod]
      public void StatusCdIsA_IfAllowUse()
      {
         var model = new SystemDataObject();

         model.AllowUse = true;

         Assert.AreEqual( 'A', model.StatusCd );
         Assert.IsTrue( model.AllowUse );
      }

      [TestMethod]
      public void AllowUseName()
      {
         var model = new SystemDataObject();

         var display = AttributeHelper.GetDisplayAttribute( () => model.AllowUse );

         Assert.IsNotNull( display );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "AllowUse", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.AllowUse ) );
      }

      [TestMethod]
      public void CopyConstructor_CopiesAllProperties()
      {
         var model = new SystemDataObject()
         {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            AllowUse = true,
            IsPredefined = true
         };

         var newModel = new SystemDataObject( model );

         Assert.AreNotSame( model, newModel );
         Assert.AreEqual( model.Id, newModel.Id );
         Assert.AreEqual( model.Name, newModel.Name );
         Assert.AreEqual( model.Description, newModel.Description );
         Assert.AreEqual( model.StatusCd, newModel.StatusCd );
         Assert.AreEqual( model.IsPredefined, newModel.IsPredefined );
      }
   }
}
