using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.WorkItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemsControllerTest
   {
      //private static Mock<IRepository<WorkItemStatus>> _workItemStatusRepository;
      //private static Mock<IRepository<WorkItemType>> _workItemTypeRepository;
      //private static MoqMockingKernel _iocKernel;

      //private Mock<IValidator<WorkItem>> _validator;
      private Mock<IRepository<WorkItem>> _workItemRepository;
      private WorkItemsController _controller;

      //private User _currentUser;
      //private Mock<IPrincipal> _principal;
      //private Mock<IIdentity> _userIdentity;
      //private static Mock<IUserRepository> _userRepository;


      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         //CreateMockIOCKernel();
         InitializeTestData();
         //CreateStaticRepositories();
         //IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         //SetupCurrentUser();
         //SetupValidator();
         SetupWorkItemRepository();

         CreateController();
      }



      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( IEnumerable<WorkItemViewModel> ) );
      }

      [TestMethod]
      public void Index_GetsAllItems()
      {
         _controller.Index();

         _workItemRepository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = WorkItems.ModelData[2];

         _workItemRepository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var view = _controller.Details( model.Id ) as ViewResult;

         _workItemRepository.Verify( x => x.Get( model.Id ), Times.Once() );

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( WorkItemViewModel ) );
         Assert.AreEqual( model.Id, ((WorkItemViewModel)view.Model).Id );
         Assert.AreEqual( model.Name, ((WorkItemViewModel)view.Model).Name );
         Assert.AreEqual( model.Description, ((WorkItemViewModel)view.Model).Description );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         _workItemRepository.Setup( x => x.Get( id ) ).Returns( null as WorkItem );

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      //[TestMethod]
      //public void CreateGet_ReturnsViewWithViewWithModel()
      //{
      //   var result = _controller.Create() as ViewResult;

      //   Assert.IsNotNull( result );
      //   var model = result.Model as WorkItemEditorViewModel;
      //   Assert.IsNotNull( model );
      //}

      //[TestMethod]
      //public void CreateGet_InitializesProjectStatusList_NothingSelected()
      //{
      //   var result = _controller.Create() as ViewResult;

      //   var model = result.Model as ProjectEditorViewModel;

      //   Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.ProjectStatuses.Count() );
      //   foreach (var item in model.ProjectStatuses)
      //   {
      //      var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
      //      Assert.AreEqual( status.Name, item.Text );
      //      Assert.IsFalse( item.Selected );
      //   }
      //}


      #region private helpers
      //private static void CreateMockIOCKernel()
      //{
      //   _iocKernel = new MoqMockingKernel();
      //}

      //private static void IntializeMapper()
      //{
      //   Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
      //   MapperConfig.RegisterMappings();
      //}

      //private static void CreateStaticRepositories()
      //{
      //   _projectStatusRepository = _iocKernel.GetMock<IRepository<ProjectStatus>>();
      //   _projectStatusRepository.Setup( x => x.GetAll() ).Returns( ProjectStatuses.ModelData );
      //   foreach (var model in ProjectStatuses.ModelData)
      //   {
      //      _projectStatusRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
      //   }
      //}

      private static void InitializeTestData()
      {
         Users.CreateTestModelData( initializeIds: true );
         WorkItemStatuses.CreateTestModelData( initializeIds: true );
         WorkItemTypes.CreateTestModelData( initializeIds: true );
         Projects.CreateTestModelData( initializeIds: true );
      }

      ICollection<KeyValuePair<string, string>> CreateStockErrorMessages()
      {
         var messages = new List<KeyValuePair<string, string>>();

         messages.Add( new KeyValuePair<string, string>( "Name", "Name is not unique" ) );
         messages.Add( new KeyValuePair<string, string>( "SomethingElse", "Another Message" ) );

         return messages;
      }

      //private WorkItemEditorViewModel CreateWorkItemEditorViewModel()
      //{
      //   return new WorkItemEditorViewModel()
      //   {
      //      Id = Guid.NewGuid(),
      //      Name = "New Work Item",
      //      Description = "This is a test",
      //      LastModifiedUserId = default( Guid ),
      //      StatusId = ProjectStatuses.ModelData[0].Id,
      //      StatusName = ProjectStatuses.ModelData[0].Name
      //   };
      //}

      //private WorkItemEditorViewModel CreateWorkItemEditorViewModel( WorkItem workItem )
      //{
      //   return new WorkItemEditorViewModel()
      //   {
      //      Id = workItem.Id,
      //      Name = workItem.Name,
      //      Description = workItem.Description,
      //      LastModifiedUserId = workItem.LastModifiedUserRid,
      //      StatusId = workItem.Status.Id,
      //      StatusName = workItem.Status.Name
      //   };
      //}

      //private void SetupCurrentUser()
      //{
      //   _userRepository = new Mock<IUserRepository>();
      //   _userIdentity = new Mock<IIdentity>();
      //   _principal = new Mock<IPrincipal>();
      //   _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

      //   _currentUser = new User()
      //   {
      //      Id = Guid.NewGuid(),
      //      UserName = "test",
      //      FirstName = "Fred"
      //   };
      //   _userRepository
      //      .Setup( x => x.Get( "test" ) )
      //      .Returns( _currentUser );
      //   _userIdentity
      //      .SetupGet( x => x.Name )
      //      .Returns( "test" );
      //}

      private void CreateController()
      {
         //_controller = new WorkItemsController( _workItemRepository.Object, _workItemStatusRepository.Object, _userRepository.Object, _validator.Object );
         _controller = new WorkItemsController( _workItemRepository.Object );
         _controller.ControllerContext = new ControllerContext();
      }

      private void SetupWorkItemRepository()
      {
         WorkItems.CreateTestModelData( initializeIds: true );
         _workItemRepository = new Mock<IRepository<WorkItem>>();
         _workItemRepository.Setup( x => x.GetAll() ).Returns( WorkItems.ModelData );
      }

      //private void SetupValidator()
      //{
      //   _validator = new Mock<IValidator<WorkItem>>();
      //   _validator.Setup( x => x.ModelIsValid( It.IsAny<WorkItem>(), It.IsAny<TransactionType>() ) ).Returns( true );
      //}
      #endregion
   }
}
