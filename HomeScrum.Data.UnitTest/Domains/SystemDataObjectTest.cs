using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;

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
   }
}
