using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Common.Utility.UnitTest
{
   [TestClass]
   public class ClassHelperTest
   {
      private string TestMethod() { return "something"; }
      private string TestData;
      private string TestProperty { get; set; }

      [TestMethod]
      public void ExtractProperyName_ThrowsException_WhenNoMemberPassed()
      {
         try
         {
            ClassHelper.ExtractPropertyName( () => this );
         }
         catch (Exception e)
         {
            var argEx = e as ArgumentException;
            Assert.IsNotNull( argEx );
            Assert.AreEqual( "propertyExpression", argEx.ParamName );
            return;
         }
         Assert.Fail( "Should have thrown exception" );
      }

      [TestMethod]
      public void ExtractProperyName_ThrowsException_WhenPassedMemberNotProperty()
      {
         try
         {
            ClassHelper.ExtractPropertyName( () => this.TestData );
         }
         catch (Exception e)
         {
            var argEx = e as ArgumentException;
            Assert.IsNotNull( argEx );
            Assert.AreEqual( "propertyExpression", argEx.ParamName );
            return;
         }
         Assert.Fail( "Should have thrown exception" );
      }

      [TestMethod]
      public void ExtractPropertyName_ReturnsPropertyName()
      {
         Assert.AreEqual( "TestProperty", ClassHelper.ExtractPropertyName( () => this.TestProperty ) );
      }
   }
}
