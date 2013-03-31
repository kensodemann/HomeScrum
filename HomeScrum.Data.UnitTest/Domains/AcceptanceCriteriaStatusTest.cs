using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class AcceptanceCriteriaStatusTest
   {
      [TestMethod]
      public void IsAcceptedDisplayName()
      {
         var model = new AcceptanceCriteriaStatus();

         var display = AttributeHelper.GetDisplayAttribute( () => model.IsAccepted );

         Assert.IsNotNull( display, "Display attribute does not exist" );
         Assert.AreEqual( typeof( DisplayStrings ), display.ResourceType );
         Assert.AreEqual( "AcceptanceCriteriaStatusIsAccepted", display.Name );
         Assert.IsFalse( String.IsNullOrWhiteSpace( DisplayStrings.AcceptanceCriteriaStatusIsAccepted ) );
      }

      [TestMethod]
      public void CopyConstructor_CopiesAllProperties()
      {
         var model = new AcceptanceCriteriaStatus()
         {
            Id = Guid.NewGuid(),
            Name = "New Name",
            Description = "New Description",
            IsPredefined = true,
            AllowUse = true,
            IsAccepted = true
         };

         var newModel = new AcceptanceCriteriaStatus( model );

         Assert.AreNotSame( model, newModel );
         Assert.AreEqual( model.Id, newModel.Id );
         Assert.AreEqual( model.Name, newModel.Name );
         Assert.AreEqual( model.Description, newModel.Description );
         Assert.AreEqual( model.StatusCd, newModel.StatusCd );
         Assert.AreEqual( model.IsPredefined, newModel.IsPredefined );
         Assert.AreEqual( model.IsAccepted, newModel.IsAccepted );
      }
   }
}
