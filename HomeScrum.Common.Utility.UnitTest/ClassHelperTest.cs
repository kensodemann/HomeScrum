using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Common.Utility.UnitTest
{
   [TestClass]
   public class ClassHelperTest
   {
      private string TestMethod() { return "something"; }
      private string TestMethod1Arg( string arg ) { return "something: " + arg; }
      private string TestField = "test";
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
      public void ExtractProperyName_ThrowsException_WhenPassedMemberIsField()
      {
         try
         {
            ClassHelper.ExtractPropertyName( () => this.TestField );
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
      public void ExtractProperyName_ThrowsException_WhenPassedMemberIsMethod()
      {
         try
         {
            ClassHelper.ExtractPropertyName( () => this.TestMethod1Arg( "test" ) );
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
      public void ExtractMethodName_ThrowsException_WhenPassedProperty()
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

      [TestMethod]
      public void ExtractMethodName_ThrowsException_WhenPassedField()
      {
         try
         {
            ClassHelper.ExtractMethodName( () => this.TestField );
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
      public void ExtractMethodName_ReturnsMethodName_NoArgs()
      {
         Assert.AreEqual( "TestMethod", ClassHelper.ExtractMethodName( () => this.TestMethod() ) );
      }

      [TestMethod]
      public void ExtractMethodName_ReturnsMethodName_WithArgs()
      {
         Assert.AreEqual( "TestMethod1Arg", ClassHelper.ExtractMethodName( () => this.TestMethod1Arg( "testit" ) ) );
      }

      [TestMethod]
      public void ExtractClassType_ReturnsClassType_ForMethod()
      {
         Assert.AreEqual( typeof( ClassHelperTest ), ClassHelper.ExtractClassType( () => this.TestMethod() ) );
      }

      [TestMethod]
      public void ExtractClassType_ReturnsClassType_ForField()
      {
         Assert.AreEqual( typeof( ClassHelperTest ), ClassHelper.ExtractClassType( () => this.TestField ) );
      }

      [TestMethod]
      public void ExtractClassType_ReturnsClassType_ForProperty()
      {
         Assert.AreEqual( typeof( ClassHelperTest ), ClassHelper.ExtractClassType( () => this.TestProperty ) );
      }
   }
}
