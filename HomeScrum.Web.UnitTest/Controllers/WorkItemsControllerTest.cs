using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.WorkItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemsControllerTest
   {
      private static Mock<IRepository<WorkItemStatus>> _workItemStatusRepository;
      private static Mock<IRepository<WorkItemType>> _workItemTypeRepository;
      private static Mock<IRepository<Project>> _projectRepository;
      private static Mock<IUserRepository> _userRepository;
      private static MoqMockingKernel _iocKernel;

      private Mock<IValidator<WorkItem>> _validator;
      private Mock<IRepository<WorkItem>> _workItemRepository;
      private WorkItemsController _controller;

      //private User _currentUser;
      //private Mock<IPrincipal> _principal;
      //private Mock<IIdentity> _userIdentity;
      //private static Mock<IUserRepository> _userRepository;


      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         CreateMockIOCKernel();
         InitializeTestData();
         CreateStaticRepositories();
         IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         //SetupCurrentUser();
         SetupValidator();
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

      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as WorkItemEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemStatusList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.Statuses.Count() );
         foreach (var item in model.Statuses)
         {
            var status = WorkItemStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemTypeList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), model.WorkItemTypes.Count() );
         foreach (var item in model.WorkItemTypes)
         {
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      [TestMethod]
      public void CreateGet_InitializesProjectList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.StatusCd == 'A' && x.Status.IsActive ) + 1, model.Projects.Count() );

         bool isFirst = true;
         foreach (var item in model.Projects)
         {
            if (isFirst)
            {
               Assert.IsNull( item.Value );
               Assert.AreEqual( DisplayStrings.NotAssigned, item.Text );
               Assert.IsFalse( item.Selected );
               isFirst = false;
            }
            else
            {
               var project = Projects.ModelData.First( x => x.Id.ToString() == item.Value );
               Assert.AreEqual( project.Name, item.Text );
               Assert.IsFalse( item.Selected );
            }
         }
      }

      [TestMethod]
      public void CreateGet_InitializesUserList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, model.Users.Count() );

         for (int i = 0; i < model.Users.Count(); i++)
         {
            var item = model.Users.ElementAt(i);
            if (i == 0)
            {
               Assert.IsNull( item.Value );
               Assert.AreEqual( DisplayStrings.NotAssigned, item.Text );
               Assert.IsFalse( item.Selected );
            }
            else
            {
               var user = Users.ModelData.First( x => x.Id.ToString() == item.Value );
               Assert.AreEqual( (String.IsNullOrWhiteSpace( user.LastName ) ? "" : user.LastName + ", ") + user.FirstName, item.Text );
               Assert.IsFalse( item.Selected );
            }
         }
      }

      // TODO: More Create tests, including create POST tests go here

      [TestMethod]
      public void EditGet_CallsRepositoryGet()
      {
         Guid id = Guid.NewGuid();
         _controller.Edit( id );

         _workItemRepository.Verify( x => x.Get( id ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithModel()
      {
         var model = WorkItems.ModelData[3];
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         var returnedModel = result.Model as WorkItemEditorViewModel;
         Assert.IsNotNull( returnedModel );
         Assert.AreEqual( model.Id, returnedModel.Id );
      }

      [TestMethod]
      public void EditGet_InitializesWorkItemStatuses_WorkItemStatusSelected()
      {
         var model = WorkItems.ModelData.First( x => x.Status.StatusCd == 'A' );
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.Statuses.Count() );
         foreach (var item in viewModel.Statuses)
         {
            var status = WorkItemStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsTrue( (model.Status.Id.ToString() != item.Value && !item.Selected) ||
                           (model.Status.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_InitializesWorkItemTypes_WorkItemTypeSelected()
      {
         var model = WorkItems.ModelData.First( x => x.WorkItemType != null && x.WorkItemType.StatusCd == 'A' );
         _workItemRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;
         var viewModel = result.Model as WorkItemEditorViewModel;

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), viewModel.WorkItemTypes.Count() );
         foreach (var item in viewModel.WorkItemTypes)
         {
            var workItemType = WorkItemTypes.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( workItemType.Name, item.Text );
            Assert.IsTrue( (model.WorkItemType.Id.ToString() != item.Value && !item.Selected) ||
                           (model.WorkItemType.Id.ToString() == item.Value && item.Selected) );
         }
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      {
         _workItemRepository.Setup( x => x.Get( It.IsAny<Guid>() ) ).Returns( null as WorkItem );

         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      //[TestMethod]
      //public void EditPost_CallRepositoryUpdateIfModelValid()
      //{
      //   var model = WorkItems.ModelData[2];
      //   var viewModel = CreateWorkItemEditorViewModel( model );

      //   _controller.Edit( viewModel, _principal.Object );

      //   _projectRepository.Verify( x => x.Update( It.Is<Project>( p => p.Id == model.Id ) ), Times.Once() );
      //}


      #region private helpers
      private static void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
      }

      private static void IntializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      private static void CreateStaticRepositories()
      {
         _workItemStatusRepository = _iocKernel.GetMock<IRepository<WorkItemStatus>>();
         _workItemStatusRepository.Setup( x => x.GetAll() ).Returns( WorkItemStatuses.ModelData );
         foreach (var model in WorkItemStatuses.ModelData)
         {
            _workItemStatusRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }

         _workItemTypeRepository = _iocKernel.GetMock<IRepository<WorkItemType>>();
         _workItemTypeRepository.Setup( x => x.GetAll() ).Returns( WorkItemTypes.ModelData );
         foreach (var model in WorkItemTypes.ModelData)
         {
            _workItemTypeRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }

         _projectRepository = _iocKernel.GetMock<IRepository<Project>>();
         _projectRepository.Setup( x => x.GetAll() ).Returns( Projects.ModelData );
         foreach (var model in Projects.ModelData)
         {
            _projectRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }

         _userRepository = _iocKernel.GetMock<IUserRepository>();
         _userRepository.Setup( x => x.GetAll() ).Returns( Users.ModelData );
         foreach (var model in Users.ModelData)
         {
            _userRepository.Setup( x => x.Get( model.Id ) ).Returns( model );
         }
      }

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
         _controller = new WorkItemsController( _workItemRepository.Object, _workItemStatusRepository.Object, _workItemTypeRepository.Object,
            _projectRepository.Object, _userRepository.Object, _validator.Object );
         _controller.ControllerContext = new ControllerContext();
      }

      private void SetupWorkItemRepository()
      {
         WorkItems.CreateTestModelData( initializeIds: true );
         _workItemRepository = new Mock<IRepository<WorkItem>>();
         _workItemRepository.Setup( x => x.GetAll() ).Returns( WorkItems.ModelData );
      }

      private void SetupValidator()
      {
         _validator = new Mock<IValidator<WorkItem>>();
         _validator.Setup( x => x.ModelIsValid( It.IsAny<WorkItem>(), It.IsAny<TransactionType>() ) ).Returns( true );
      }
      #endregion
   }
}
