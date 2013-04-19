using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using System;
using System.Linq;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class DomainMappingTest
   {
      #region Initialization
      [ClassInitialize]
      public static void InitializeClass( TestContext context )
      {
         AcceptanceCriteriaStatuses.CreateTestModelData( initializeIds: true );
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         SprintStatuses.CreateTestModelData( initializeIds: true );
         WorkItemStatuses.CreateTestModelData( initializeIds: true );
         WorkItemTypes.CreateTestModelData( initializeIds: true );

         Projects.CreateTestModelData( initializeIds: true );
         Users.CreateTestModelData( initializeIds: true );

         _iocKernel = new MoqMockingKernel();
         _projectStatusRepository = _iocKernel.GetMock<IRepository<ProjectStatus>>();

         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      private static Mock<IRepository<ProjectStatus>> _projectStatusRepository;
      private static MoqMockingKernel _iocKernel;
      #endregion


      [TestMethod]
      public void MapperConfigurationIsValid()
      {
         Mapper.AssertConfigurationIsValid();
      }

      #region Acceptance Criteria Status
      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_DomainToViewModel()
      {
         var domainModel = AcceptanceCriteriaStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map<DisplayViewModel>( domainModel );

         Assert.IsInstanceOfType( viewModel, typeof( AcceptanceCriteriaStatusViewModel ) );
         Assert.IsTrue( ((AcceptanceCriteriaStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_DomainToEditorViewModel()
      {
         var domainModel = AcceptanceCriteriaStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( AcceptanceCriteriaStatusEditorViewModel ) );

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
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( AcceptanceCriteriaStatus ) );

         Assert.IsInstanceOfType( domainModel, typeof( AcceptanceCriteriaStatus ) );
         Assert.AreEqual( viewModel.Id, ((AcceptanceCriteriaStatus)domainModel).Id );
         Assert.AreEqual( viewModel.Name, ((AcceptanceCriteriaStatus)domainModel).Name );
         Assert.AreEqual( viewModel.Description, ((AcceptanceCriteriaStatus)domainModel).Description );
         Assert.AreEqual( 'A', ((AcceptanceCriteriaStatus)domainModel).StatusCd );
      }
      #endregion

      #region Project Status
      [TestMethod]
      public void CanMapProjectStatus_DomainToViewModel()
      {
         var domainModel = ProjectStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( ProjectStatusViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( ProjectStatusViewModel ) );
         Assert.IsTrue( ((ProjectStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapProjectStatus_DomainToEditorViewModel()
      {
         var domainModel = ProjectStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( ProjectStatusEditorViewModel ) );

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
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( ProjectStatus ) );

         Assert.IsInstanceOfType( domainModel, typeof( ProjectStatus ) );
         Assert.AreEqual( 'A', ((ProjectStatus)domainModel).StatusCd );
      }
      #endregion

      #region Sprint Status
      [TestMethod]
      public void CanMapSprintStatus_DomainToViewModel()
      {
         var domainModel = SprintStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( SprintStatusViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( SprintStatusViewModel ) );
         Assert.IsTrue( ((SprintStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapSprintStatus_DomainToEditorViewModel()
      {
         var domainModel = SprintStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( SprintStatusEditorViewModel ) );

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
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( SprintStatus ) );

         Assert.IsInstanceOfType( domainModel, typeof( SprintStatus ) );
         Assert.AreEqual( 'A', ((SprintStatus)domainModel).StatusCd );
      }
      #endregion

      #region Work Item Status
      [TestMethod]
      public void CanMapWorkItemStatus_DomainToViewModel()
      {
         var domainModel = WorkItemStatuses.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( WorkItemStatusViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemStatusViewModel ) );
         Assert.IsTrue( ((WorkItemStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapWorkItemStatus_DomainToEditorViewModel()
      {
         var domainModel = WorkItemStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( WorkItemStatusEditorViewModel ) );

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
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( WorkItemStatus ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItemStatus ) );
         Assert.AreEqual( 'A', ((WorkItemStatus)domainModel).StatusCd );
      }
      #endregion

      #region Work Item Type
      [TestMethod]
      public void CanMapWorkItemType_DomainToViewModel()
      {
         var domainModel = WorkItemTypes.ModelData.ToArray().First( x => x.StatusCd == 'A' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( WorkItemTypeViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemTypeViewModel ) );
         Assert.IsTrue( ((WorkItemTypeViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapWorkItemType_DomainToEditorViewModel()
      {
         var domainModel = WorkItemTypes.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( WorkItemTypeEditorViewModel ) );

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
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( WorkItemType ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItemType ) );
         Assert.AreEqual( 'A', ((WorkItemType)domainModel).StatusCd );
      }
      #endregion


      #region Project
      [TestMethod]
      public void CanMapProject_DomainToViewModel()
      {
         var domainModel = Projects.ModelData[0];
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( ProjectViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( ProjectViewModel ) );
         Assert.AreEqual( domainModel.Status.Name, ((ProjectViewModel)viewModel).StatusName );
      }


      [TestMethod]
      public void CanMapProject_DomainToEditorViewModel()
      {
         var domainModel = Projects.ModelData.ToArray()[0];
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( ProjectEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( ProjectEditorViewModel ) );
         Assert.AreEqual( domainModel.Status.Id, ((ProjectEditorViewModel)viewModel).StatusId );
         Assert.AreEqual( domainModel.Status.Name, ((ProjectEditorViewModel)viewModel).StatusName );
      }

      [TestMethod]
      public void CanMapProject_EditorViewModelToDomain()
      {
         var viewModel = new ProjectEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            LastModifiedUserId = Users.ModelData[0].Id,
            StatusId = ProjectStatuses.ModelData[0].Id
         };
         _projectStatusRepository
            .Setup( x => x.Get( ProjectStatuses.ModelData[0].Id ) )
            .Returns( ProjectStatuses.ModelData[0] )
            .Verifiable();

         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( Project ) );

         Assert.IsInstanceOfType( domainModel, typeof( Project ) );
         Assert.AreEqual( ProjectStatuses.ModelData[0], ((Project)domainModel).Status );
      }
      #endregion
   }
}
