using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Models.WorkItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
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


         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );

         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( _sessionFactory.Object );

         Database.Build( _session );

         Users.Load( _sessionFactory.Object );

         AcceptanceCriteriaStatuses.Load( _sessionFactory.Object );
         ProjectStatuses.Load( _sessionFactory.Object );
         SprintStatuses.Load( _sessionFactory.Object );
         WorkItemStatuses.Load( _sessionFactory.Object );
         WorkItemTypes.Load( _sessionFactory.Object );

         Projects.Load( _sessionFactory.Object );
         WorkItems.Load( _sessionFactory.Object );
      }

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;
      private static MoqMockingKernel _iocKernel;
      #endregion


      [TestMethod]
      public void MapperConfigurationIsValid()
      {
         Mapper.AssertConfigurationIsValid();
      }

      #region Acceptance Criteria Status
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
            Category = AcceptanceCriterionStatusCategory.VerificationPassed,
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
            Category = ProjectStatusCategory.Active,
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
      public void CanMapSprintStatus_DomainToEditorViewModel()
      {
         var domainModel = SprintStatuses.ModelData.ToArray().First( x => x.StatusCd == 'I' );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( SprintStatusEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( SprintStatusEditorViewModel ) );
         Assert.IsFalse( ((SprintStatusEditorViewModel)viewModel).AllowUse );
         Assert.AreEqual( ((SprintStatusEditorViewModel)viewModel).CanAddBacklogItems, !domainModel.BacklogIsClosed );
         Assert.AreEqual( ((SprintStatusEditorViewModel)viewModel).CanAddTaskListItems, !domainModel.TaskListIsClosed );
      }

      [TestMethod]
      public void CanMapSprintStatus_EditorViewModelToDomain()
      {
         var viewModel = new SprintStatusEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test",
            Category = SprintStatusCategory.Active,
            IsPredefined = false,
            AllowUse = true,
            CanAddBacklogItems = false,
            CanAddTaskListItems = true
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( SprintStatus ) );

         Assert.IsInstanceOfType( domainModel, typeof( SprintStatus ) );
         Assert.AreEqual( 'A', ((SprintStatus)domainModel).StatusCd );
         Assert.IsTrue( ((SprintStatus)domainModel).BacklogIsClosed );
         Assert.IsFalse( ((SprintStatus)domainModel).TaskListIsClosed );
      }
      #endregion


      #region Work Item Status
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
            Category = WorkItemStatusCategory.InProcess,
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
            Category = WorkItemTypeCategory.Task,
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


      #region Sprint
      [TestMethod]
      public void CanMapSprint_DomainToEditorViewModel()
      {
         var domainModel = Sprints.ModelData[0];
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( SprintEditorViewModel ) );

         Assert.IsInstanceOfType( viewModel, typeof( SprintEditorViewModel ) );
         Assert.AreEqual( domainModel.Project.Id, ((SprintEditorViewModel)viewModel).ProjectId );
         Assert.AreEqual( domainModel.Project.Name, ((SprintEditorViewModel)viewModel).ProjectName );
         Assert.AreEqual( domainModel.Status.Id, ((SprintEditorViewModel)viewModel).StatusId );
         Assert.AreEqual( domainModel.Status.Name, ((SprintEditorViewModel)viewModel).StatusName );
         Assert.AreEqual( domainModel.Goal, ((SprintEditorViewModel)viewModel).Goal );
      }

      [TestMethod]
      public void CanMapSprint_EditorViewModelToDomain()
      {
         var viewModel = new SprintEditorViewModel()
         {
            Id = Guid.NewGuid(),
            Name = "Test Me",
            Description = "This is a test.",
            StartDate = new DateTime( 2013, 3, 1 ),
            EndDate = new DateTime( 2013, 3, 31 ),
            Goal = "Get this thing tested",
            ProjectId = Projects.ModelData[2].Id,
            StatusId = SprintStatuses.ModelData[1].Id
         };
         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( Sprint ) );

         Assert.AreEqual( viewModel.Id, ((Sprint)domainModel).Id );
         Assert.AreEqual( viewModel.Name, ((Sprint)domainModel).Name );
         Assert.AreEqual( viewModel.Description, ((Sprint)domainModel).Description );
         Assert.AreEqual( viewModel.Goal, ((Sprint)domainModel).Goal );
         Assert.AreEqual( viewModel.StartDate, ((Sprint)domainModel).StartDate );
         Assert.AreEqual( viewModel.EndDate, ((Sprint)domainModel).EndDate );
         Assert.AreEqual( viewModel.StatusId, ((Sprint)domainModel).Status.Id );
         Assert.AreEqual( viewModel.ProjectId, ((Sprint)domainModel).Project.Id );
      }

      [TestMethod]
      public void Sprint_EditorViewModelToDomain_IncludesCalendar()
      {
         var domainModel = Sprints.ModelData.First( x => x.Calendar.Count() > 0 );
         var viewModel = Mapper.Map( domainModel, domainModel.GetType(), typeof( SprintEditorViewModel ) );
         var mappedModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( Sprint ) ) as Sprint;

         Assert.AreEqual( domainModel.Calendar.Count(), mappedModel.Calendar.Count() );
      }
      #endregion


      #region WorkItem
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
            ParentWorkItemName = WorkItems.ModelData[0].Name,
            SprintId = Sprints.ModelData[1].Id,
            SprintName = Sprints.ModelData[1].Name
         };

         var domainModel = Mapper.Map( viewModel, viewModel.GetType(), typeof( WorkItem ) );

         Assert.IsInstanceOfType( domainModel, typeof( WorkItem ) );
         AssertDomainModelsEqual( WorkItemStatuses.ModelData[0], ((WorkItem)domainModel).Status );
         AssertDomainModelsEqual( WorkItemTypes.ModelData[1], ((WorkItem)domainModel).WorkItemType );
         AssertDomainModelsEqual( Projects.ModelData[0], ((WorkItem)domainModel).Project );
         AssertUsersEqual( Users.ModelData[0], ((WorkItem)domainModel).AssignedToUser );
         AssertUsersEqual( Users.ModelData[1], ((WorkItem)domainModel).CreatedByUser );
         AssertDomainModelsEqual( WorkItems.ModelData[0], ((WorkItem)domainModel).ParentWorkItem );
         AssertDomainModelsEqual( Sprints.ModelData[1], ((WorkItem)domainModel).Sprint );
      }
      #endregion


      #region Private Helpers
      private void AssertDomainModelsEqual( DomainObjectBase expected, DomainObjectBase actual )
      {
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
      }

      private void AssertUsersEqual( User expected, User actual )
      {
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.FirstName, actual.FirstName );
         Assert.AreEqual( expected.LastName, actual.LastName );
         Assert.AreEqual( expected.UserName, actual.UserName );
      }
      #endregion
   }
}
