using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Common.Utility.UnitTest
{
   public class TestClassBase
   {
      public virtual string TestMethod() { return "something"; }
      public virtual string TestMethod1Arg( string arg ) { return "something: " + arg; }
      public string TestField = "test";
      public virtual string TestProperty { get; set; }
   }

   public class TestClass : TestClassBase { }


   [TestClass]
   public class ClassHelperTest
   {
      

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
         var model = new TestClass();

         try
         {
            ClassHelper.ExtractPropertyName( () => model.TestField );
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
         var model = new TestClass();
         
         try
         {
            ClassHelper.ExtractPropertyName( () => model.TestMethod1Arg( "test" ) );
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
         var model = new TestClass();
         
         Assert.AreEqual( "TestProperty", ClassHelper.ExtractPropertyName( () => model.TestProperty ) );
      }


      [TestMethod]
      public void ExtractMethodName_ThrowsException_WhenPassedProperty()
      {
         var model = new TestClass();
         
         try
         {
            ClassHelper.ExtractMethodName( () => model.TestProperty );
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
         var model = new TestClass();
         
         try
         {
            ClassHelper.ExtractMethodName( () => model.TestField );
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
         var model = new TestClass();
         
         Assert.AreEqual( "TestMethod", ClassHelper.ExtractMethodName( () => model.TestMethod() ) );
      }

      [TestMethod]
      public void ExtractMethodName_ReturnsMethodName_WithArgs()
      {
         var model = new TestClass();
         
         Assert.AreEqual( "TestMethod1Arg", ClassHelper.ExtractMethodName( () => model.TestMethod1Arg( "testit" ) ) );
      }

      [TestMethod]
      public void ExtractClassType_ReturnsClassType_ForMethod()
      {
         var model = new TestClass();
         
         Assert.AreEqual( typeof( TestClass ), ClassHelper.ExtractClassType( () => model.TestMethod() ) );
      }

      [TestMethod]
      public void ExtractClassType_ReturnsClassType_ForField()
      {
         var model = new TestClass();

         Assert.AreEqual( typeof( TestClass ), ClassHelper.ExtractClassType( () => model.TestField ) );
      }

      [TestMethod]
      public void ExtractClassType_ReturnsClassType_ForProperty()
      {
         var model = new TestClass();

         Assert.AreEqual( typeof( TestClass ), ClassHelper.ExtractClassType( () => model.TestProperty ) );
      }
   }
}
