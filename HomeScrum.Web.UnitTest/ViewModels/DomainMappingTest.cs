﻿using System;
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
      #region Initialization
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
      #endregion

      #region Acceptance Criteria Status
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

         Assert.IsInstanceOfType( domainModel, typeof( AcceptanceCriteriaStatus ) );
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
      #endregion

      #region Project Status
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
      #endregion

      #region Sprint Status
      [TestMethod]
      public void CanMapSprintStatus_DomainToViewModel()
      {
         var domainModel = SprintStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( SprintStatusViewModel ) );
         Assert.IsTrue( ((SprintStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapSprintStatus_ViewModelToDomain()
      {
         var viewModel = new SprintStatusViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsOpenStatus = true,
            IsPredefined = false,
            AllowUse = false
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( SprintStatus ) );
         Assert.AreEqual( 'I', ((SprintStatus)domainModel).StatusCd );
      }

      [TestMethod]
      public void CanMapSprintStatus_DomainToEditorViewModel()
      {
         var domainModel = SprintStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( SprintStatusEditorViewModel ) );
         Assert.IsFalse( ((SprintStatusEditorViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapSprintStatus_EditorViewModelToDomain()
      {
         var viewModel = new SprintStatusEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsOpenStatus = true,
            IsPredefined = false,
            AllowUse = true
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( SprintStatus ) );
         Assert.AreEqual( 'A', ((SprintStatus)domainModel).StatusCd );
      }
      #endregion

      #region Work Item Status
      [TestMethod]
      public void CanMapWorkItemStatus_DomainToViewModel()
      {
         var domainModel = WorkItemStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemStatusViewModel ) );
         Assert.IsTrue( ((WorkItemStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapWorkItemStatus_ViewModelToDomain()
      {
         var viewModel = new WorkItemStatusViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsOpenStatus = true,
            IsPredefined = false,
            AllowUse = false
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItemStatus ) );
         Assert.AreEqual( 'I', ((WorkItemStatus)domainModel).StatusCd );
      }

      [TestMethod]
      public void CanMapWorkItemStatus_DomainToEditorViewModel()
      {
         var domainModel = WorkItemStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemStatusEditorViewModel ) );
         Assert.IsFalse( ((WorkItemStatusEditorViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapWorkItemStatus_EditorViewModelToDomain()
      {
         var viewModel = new WorkItemStatusEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsOpenStatus = true,
            IsPredefined = false,
            AllowUse = true
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItemStatus ) );
         Assert.AreEqual( 'A', ((WorkItemStatus)domainModel).StatusCd );
      }
      #endregion

      #region Work Item Type
      [TestMethod]
      public void CanMapWorkItemType_DomainToViewModel()
      {
         var domainModel = WorkItemTypes.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemTypeViewModel ) );
         Assert.IsTrue( ((WorkItemTypeViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapWorkItemType_ViewModelToDomain()
      {
         var viewModel = new WorkItemTypeViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsTask = true,
            IsPredefined = false,
            AllowUse = false
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItemType ) );
         Assert.AreEqual( 'I', ((WorkItemType)domainModel).StatusCd );
      }

      [TestMethod]
      public void CanMapWorkItemType_DomainToEditorViewModel()
      {
         var domainModel = WorkItemTypes.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( DomainObjectEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemTypeEditorViewModel ) );
         Assert.IsFalse( ((WorkItemTypeEditorViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapWorkItemType_EditorViewModelToDomain()
      {
         var viewModel = new WorkItemTypeEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsTask = true,
            IsPredefined = false,
            AllowUse = true
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( DomainObjectBase ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItemType ) );
         Assert.AreEqual( 'A', ((WorkItemType)domainModel).StatusCd );
      }
      #endregion
   }
}
