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
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         SprintStatuses.CreateTestModelData( initializeIds: true );
         WorkItemStatuses.CreateTestModelData( initializeIds: true );
         WorkItemTypes.CreateTestModelData( initializeIds: true );
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

      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_DomainToEditorViewModel()
      {
         var domainModel = AcceptanceCriteriaStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( AcceptanceCriteriaStatusEditorViewModel ) );
         Assert.IsFalse( ((AcceptanceCriteriaStatusEditorViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_EditorViewModelToDomain()
      {
         var viewModel = new AcceptanceCriteriaStatusEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsAccepted = true,
            IsPredefined = false,
            AllowUse = true
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( AcceptanceCriteriaStatus ) );
         Assert.AreEqual( 'A', ((AcceptanceCriteriaStatus)domainModel).StatusCd );
      }

      [TestMethod]
      public void CanMapProjectStatus_DomainToViewModel()
      {
         var domainModel = ProjectStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( ProjectStatusViewModel ) );
         Assert.IsTrue( ((ProjectStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapProjectStatus_ViewModelToDomain()
      {
         var viewModel = new ProjectStatusViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsActive = true,
            IsPredefined = false,
            AllowUse = false
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( ProjectStatus ) );
         Assert.AreEqual( 'I', ((ProjectStatus)domainModel).StatusCd );
      }

      [TestMethod]
      public void CanMapProjectStatus_DomainToEditorViewModel()
      {
         var domainModel = ProjectStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( ProjectStatusEditorViewModel ) );
         Assert.IsFalse( ((ProjectStatusEditorViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapProjectStatus_EditorViewModelToDomain()
      {
         var viewModel = new ProjectStatusEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsActive = true,
            IsPredefined = false,
            AllowUse = true
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( ProjectStatus ) );
         Assert.AreEqual( 'A', ((ProjectStatus)domainModel).StatusCd );
      }
   }
}
