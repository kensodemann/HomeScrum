using System;
using System.Collections.Generic;
using System.Security.Principal;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
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
      //private WorkItemsController _controller;

      //private User _currentUser;
      //private Mock<IPrincipal> _principal;
      //private Mock<IIdentity> _userIdentity;
      //private static Mock<IUserRepository> _userRepository;


      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         //CreateMockIOCKernel();
         //InitializeTestData();
         //CreateStaticRepositories();
         //IntializeMapper();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         //SetupCurrentUser();
         //SetupValidator();
         SetupWorkItemRepository();

         //CreateController();
      }



      [TestMethod]
      public void TestMethod1()
      {
      }


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

      //private static void InitializeTestData()
      //{
      //   Users.CreateTestModelData( initializeIds: true );
      //   WorkItemStatuses.CreateTestModelData( initializeIds: true );
      //   WorkItemTypes.CreateTestModelData( initializeIds: true );
      //   Projects.CreateTestModelData( initializeIds: true );
      //}

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

      //private void CreateController()
      //{
      //   _controller = new WorkItemsController( _projectRepository.Object, _projectStatusRepository.Object, _userRepository.Object, _validator.Object );
      //   _controller.ControllerContext = new ControllerContext();
      //}

      private void SetupWorkItemRepository()
      {
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
