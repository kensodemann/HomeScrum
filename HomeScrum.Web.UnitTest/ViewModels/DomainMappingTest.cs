using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.WorkItems;
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
         Database.Initialize();

         _iocKernel = new MoqMockingKernel();
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         Database.Build();

         Users.Load();

         AcceptanceCriteriaStatuses.Load();
         ProjectStatuses.Load();
         SprintStatuses.Load();
         WorkItemStatuses.Load();
         WorkItemTypes.Load();

         Projects.Load();
         WorkItems.Load();
      }

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
         var viewModel = Mapper.Map<AcceptanceCriterionStatusViewModel>( domainModel );

         Assert.IsInstanceOfType( viewModel, typeof( AcceptanceCriterionStatusViewModel ) );
         Assert.IsTrue( ((AcceptanceCriterionStatusViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_DomainToEditorViewModel()
      {
         var domainModel = AcceptanceCriteriaStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( AcceptanceCriterionStatusEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( AcceptanceCriterionStatusEditorViewModel ) );
         Assert.IsFalse( ((AcceptanceCriterionStatusEditorViewModel)viewModel).AllowUse );
      }

      [TestMethod]
      public void CanMapAcceptanceCriteriaStatus_EditorViewModelToDomain()
      {
         var viewModel = new AcceptanceCriterionStatusEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            IsAccepted = true,
            IsPredefined = false,
            AllowUse = true
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( AcceptanceCriterionStatus ) );

         Assert.IsInstanceOfType( domainModel, typeof( AcceptanceCriterionStatus ) );
         Assert.AreEqual( viewModel.Id, ((AcceptanceCriterionStatus)domainModel).Id );
         Assert.AreEqual( viewModel.Name, ((AcceptanceCriterionStatus)domainModel).Name );
         Assert.AreEqual( viewModel.Description, ((AcceptanceCriterionStatus)domainModel).Description );
         Assert.AreEqual( 'A', ((AcceptanceCriterionStatus)domainModel).StatusCd );
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

         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( Project ) );

         Assert.IsInstanceOfType( domainModel, typeof( Project ) );
         AssertDomainModelsEqual( ProjectStatuses.ModelData[0], ((Project)domainModel).Status );
      }
      #endregion

      #region WorkItem
      [TestMethod]
      public void CanMapWorkItem_DomainToViewModel()
      {
         var domainModel = WorkItems.ModelData.First( x => x.AcceptanceCriteria != null && x.AcceptanceCriteria.Count() > 0 );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( WorkItemViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemViewModel ) );
         Assert.AreEqual( domainModel.Status.Name, ((WorkItemViewModel)viewModel).StatusName );
         Assert.AreEqual( domainModel.WorkItemType.Name, ((WorkItemViewModel)viewModel).WorkItemTypeName );
         Assert.AreEqual( domainModel.Project.Name, ((WorkItemViewModel)viewModel).ProjectName );
         Assert.AreEqual( !domainModel.Status.IsOpenStatus, ((WorkItemViewModel)viewModel).IsComplete );
      }


      [TestMethod]
      public void CanMapWorkItem_DomainToEditorViewModel()
      {
         var domainModel = WorkItems.ModelData.ToArray()[0];
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( WorkItemEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( WorkItemEditorViewModel ) );
         Assert.AreEqual( domainModel.Status.Id, ((WorkItemEditorViewModel)viewModel).StatusId );
         Assert.AreEqual( domainModel.Status.Name, ((WorkItemEditorViewModel)viewModel).StatusName );
         Assert.AreEqual( domainModel.WorkItemType.Id, ((WorkItemEditorViewModel)viewModel).WorkItemTypeId );
         Assert.AreEqual( domainModel.WorkItemType.Name, ((WorkItemEditorViewModel)viewModel).WorkItemTypeName );
         Assert.AreEqual( domainModel.Project.Id, ((WorkItemEditorViewModel)viewModel).ProjectId );
         Assert.AreEqual( domainModel.Project.Name, ((WorkItemEditorViewModel)viewModel).ProjectName );
         Assert.AreEqual( domainModel.CreatedByUser.UserName, ((WorkItemEditorViewModel)viewModel).CreatedByUserUserName );
         if (domainModel.AssignedToUser != null)
         {
            Assert.AreEqual( domainModel.AssignedToUser.UserName, ((WorkItemEditorViewModel)viewModel).AssignedToUserUserName );
         }
      }

      [TestMethod]
      public void CanMapWorkItem_EditorViewModelToDomain()
      {
         var viewModel = new WorkItemEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            StatusId = WorkItemStatuses.ModelData[0].Id,
            StatusName = WorkItemStatuses.ModelData[0].Name,
            WorkItemTypeId = WorkItemTypes.ModelData[1].Id,
            WorkItemTypeName = WorkItemTypes.ModelData[1].Name,
            ProjectId = Projects.ModelData[0].Id,
            ProjectName = Projects.ModelData[0].Name,
            AssignedToUserId = Users.ModelData[0].Id,
            AssignedToUserUserName = Users.ModelData[0].UserName,
            CreatedByUserId = Users.ModelData[1].Id,
            CreatedByUserUserName = Users.ModelData[1].UserName,
            ParentWorkItemId = WorkItems.ModelData[0].Id,
            ParentWorkItemName = WorkItems.ModelData[0].Name
         };

         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( WorkItem ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItem ) );
         AssertDomainModelsEqual( WorkItemStatuses.ModelData[0], ((WorkItem)domainModel).Status );
         AssertDomainModelsEqual( WorkItemTypes.ModelData[1], ((WorkItem)domainModel).WorkItemType );
         AssertDomainModelsEqual( Projects.ModelData[0], ((WorkItem)domainModel).Project );
         AssertUsersEqual( Users.ModelData[0], ((WorkItem)domainModel).AssignedToUser );
         AssertUsersEqual( Users.ModelData[1], ((WorkItem)domainModel).CreatedByUser );
         AssertDomainModelsEqual( WorkItems.ModelData[0], ((WorkItem)domainModel).ParentWorkItem );
      }
      #endregion


      #region Private Helpers
      private void AssertDomainModelsEqual( DomainObjectBase expected, DomainObjectBase actual )
      {
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
      }

      private void AssertUsersEqual(User expected, User actual)
      {
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.FirstName, actual.FirstName );
         Assert.AreEqual( expected.LastName, actual.LastName );
         Assert.AreEqual( expected.UserName, actual.UserName );
      }
      #endregion
   }
}
