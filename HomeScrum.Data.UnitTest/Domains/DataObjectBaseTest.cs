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
      public void NameIsRequired()
      {
         var model = new DomainObjectBase();

         var required = AttributeHelper.GetRequiredAttribute( () => model.Name );

         Assert.IsNotNull( required );
         Assert.AreEqual( "NameIsRequired", required.ErrorMessageResourceName );
         Assert.AreEqual( typeof( ErrorMessages ), required.ErrorMessageResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( ErrorMessages.NameIsRequired ) );
      }

      [TestMethod]
      public void NameHasMaxLength()
      {
         var model = new DomainObjectBase();

         var maxLength = AttributeHelper.GetMaxLengthAttribute( () => model.Name );

         Assert.IsNotNull( maxLength );
         Assert.AreEqual( "NameMaxLength", maxLength.ErrorMessageResourceName );
         Assert.AreEqual( typeof( ErrorMessages ), maxLength.ErrorMessageResourceType );
         Assert.IsFalse( String.IsNullOrWhiteSpace( ErrorMessages.NameIsRequired ) );
      }
   }
}
