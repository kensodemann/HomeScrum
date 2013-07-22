using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Common.Test.Utility;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class DataObjectBaseTest
   {
      [TestMethod]
      public void IsNotValid_IfNameNotSpecified()
      {
         var model = new DomainObjectBase( null )
         {
            Id = Guid.NewGuid(),
            Description = "This is a description"
         };

         Assert.IsFalse( model.IsValidFor( TransactionType.All ) );
         var messages = model.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( ErrorMessages.NameIsRequired, messages["Name"] );
      }

      [TestMethod]
      public void IsNotValid_IfNameIsTooLong()
      {
         var model = new DomainObjectBase( null )
         {
            Id = Guid.NewGuid(),
            Name = "012345678901234567890123456789012345678901234567890",
            Description = "This is a description"
         };

         Assert.IsFalse( model.IsValidFor( TransactionType.All ) );
         var messages = model.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( ErrorMessages.NameMaxLength, messages["Name"] );
      }

      [TestMethod]
      public void IsValid_IfEverythingIsValid()
      {
         var model = new DomainObjectBase( null )
         {
            Id = Guid.NewGuid(),
            Name = "01234567890123456789012345678901234567890123456789",
            Description = "This is a description"
         };

         Assert.IsTrue( model.IsValidFor( TransactionType.All ) );
         var messages = model.GetErrorMessages();
         Assert.AreEqual( 0, messages.Count );
      }

   }
}
