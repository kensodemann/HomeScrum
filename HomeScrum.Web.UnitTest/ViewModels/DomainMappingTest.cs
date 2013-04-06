using System;
using System.Linq;
using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models;
using HomeScrum.Web.Models.Base;
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
         AcceptanceCriteriaStatuses.CreateTestModelData( initializeIds: true );
      }


      [TestMethod]
      public void MapperConfigurationIsValid()
      {
         Mapper.AssertConfigurationIsValid();
      }

      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_DomainToViewModel()
      {
         var domainModel = AcceptanceCriteriaStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( AcceptanceCriteriaStatusViewModel ) );
         Assert.IsTrue( ((AcceptanceCriteriaStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_ViewModelToDomain()
      {
         var viewModel = new AcceptanceCriteriaStatusViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsAccepted = true,
            IsPredefined = false,
            AllowUse = false
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof(AcceptanceCriteriaStatus) );
         Assert.AreEqual( 'I', ((AcceptanceCriteriaStatus)domainModel).StatusCd );
      }
   }
}
