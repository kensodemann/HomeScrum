using System;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class DomainMappingTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext context )
      {
         MapperConfig.RegisterMappings();
      }

      [TestMethod]
      public void MapperConfigurationIsValid()
      {
         Mapper.AssertConfigurationIsValid();
      }
   }
}
