using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.Sprints;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.MockingKernel.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class SprintsControllerTest
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private Mock<ILogger> _logger;

      private Mock<ControllerContext> _controllerConext;
      private Stack<NavigationData> _navigationStack;

      private Mock<IPrincipal> _principal;
      private Mock<IIdentity> _userIdentity;

      private SprintsController _controller;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
         InitializeMapper();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         BuildMocks();
         CreateMockIOCKernel();
         SetupSession();
         BuildDatabase();
         SetupControllerContext();
         SetupCurrentUser();

         CreateController();
      }

      private void BuildMocks()
      {
         _sessionFactory = new Mock<ISessionFactory>();
         _logger = new Mock<ILogger>();
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      private void BuildDatabase()
      {
         Database.Build( _session );
         Sprints.Load( _sessionFactory.Object );
      }

      private void CreateController()
      {
         _controller = new SprintsController( new PropertyNameTranslator<Sprint, SprintEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         _controller.ControllerContext = _controllerConext.Object;
      }

      private void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( _sessionFactory.Object );
      }

      private static void InitializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      private void SetupControllerContext()
      {
         _controllerConext = new Mock<ControllerContext>();
         _controllerConext
            .SetupSet( x => x.HttpContext.Session["NavigationStack"] = It.IsAny<Stack<NavigationData>>() )
            .Callback( ( string name, object m ) => { _navigationStack = (Stack<NavigationData>)m; } );
         _controllerConext
            .Setup( x => x.HttpContext.Session["NavigationStack"] )
            .Returns( () => _navigationStack );
      }

      private void SetupCurrentUser()
      {
         _userIdentity = new Mock<IIdentity>();
         _principal = new Mock<IPrincipal>();
         _principal.SetupGet( x => x.Identity ).Returns( _userIdentity.Object );

         // In other places where we use a random user, we use the first active one.
         // Use the first inactive user here just to ensure it is a different user.
         var currentUser = Users.ModelData.First( x => x.StatusCd == 'I' );
         _userIdentity
            .SetupGet( x => x.Name )
            .Returns( currentUser.UserName );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion

      #region Index
      [TestMethod]
      public void Index_ReturnsViewWithAllItems()
      {
         var view = _controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<DomainObjectViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( model );
         Assert.AreEqual( Sprints.ModelData.Count(), model.Count() );

         foreach (var sprint in Sprints.ModelData)
         {
            Assert.IsNotNull( model.FirstOrDefault( x => x.Id == sprint.Id ) );
         }
      }
      #endregion

      #region Create GET
      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as SprintEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesWorkItemStatusList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as SprintEditorViewModel;

         Assert.AreEqual( SprintStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.Statuses.Count() );
         foreach (var item in model.Statuses)
         {
            var status = SprintStatuses.ModelData.First( x => x.Id == new Guid( item.Value ) );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }
      #endregion

      #region Details GET
      [TestMethod]
      public void DetailsGet_ReturnsViewWithModel()
      {
         var model = Sprints.ModelData[2];

         var view = _controller.Details( model.Id ) as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( SprintViewModel ) );
         Assert.AreEqual( model.Id, ((SprintViewModel)view.Model).Id );
         Assert.AreEqual( model.Name, ((SprintViewModel)view.Model).Name );
         Assert.AreEqual( model.Description, ((SprintViewModel)view.Model).Description );
      }

      [TestMethod]
      public void DetailsGet_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void DetailsGet_AddsCallingActionAndId_IfSpecified()
      {
         var id = Sprints.ModelData[2].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.Details( id, "Edit", parentId.ToString() )).Model as SprintViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_LeavesCallingActionAndIdAsDefault_IfNotSpecified()
      {
         var id = Sprints.ModelData[2].Id;

         var viewModel = ((ViewResult)_controller.Details( id )).Model as SprintViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, "Index" );
         var viewModel = ((ViewResult)_controller.Details( id, "Edit", parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, "Index" );
         _controller.Details( id, "Edit", parentId.ToString() );
         _controller.Details( id, "Edit", parentId.ToString() );
         _controller.Details( id, "Index" );
         _controller.Details( id, "Index" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 3, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void DetailsGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var id = Sprints.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, "Index" );
         _controller.Details( id, "Edit", parentId.ToString() );
         var viewModel = ((ViewResult)_controller.Details( id )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }
      #endregion
   }
}
