using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Common.Utility.UnitTest
{
   [TestClass]
   public class ClassHelperTest
   {
      private string TestMethod() { return "something"; }
      private string TestMethod1Arg( string arg ) { return "something: " + arg; }
      private string TestData = "test";
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
            Assert.AreEqual( "expression", argEx.ParamName );
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
            Assert.AreEqual( "expression", argEx.ParamName );
            return;
         }
         Assert.Fail( "Should have thrown exception" );
      }

      [TestMethod]
      public void ExtractPropertyName_ReturnsPropertyName()
      {
         Assert.AreEqual( "TestProperty", ClassHelper.ExtractPropertyName( () => this.TestProperty ) );
      }

      [TestMethod]
      public void ExtractMethodName_ThrowsException_WhenPassedNonMethod()
      {
         try
         {
            ClassHelper.ExtractMethodName( () => this.TestProperty );
         }
         catch (Exception e)
         {
            var argEx = e as ArgumentException;
            Assert.IsNotNull( argEx );
            Assert.AreEqual( "expression", argEx.ParamName );
            return;
         }
         Assert.Fail( "Should have thrown exception" );
      }
   }
}
