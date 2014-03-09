using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Context;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class UsersControllerTest
   {
      #region Setup and Teardown
      private Mock<ISecurityService> _securityService;
      private UsersController _controller;

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;

      private Mock<ControllerContext> _controllerContext;
      private Stack<NavigationData> _navigationStack;

      private Mock<ILogger> _logger;

      private EditUserViewModel CreateNewEditViewModel( User model )
      {
         return new EditUserViewModel()
         {
            Id = model.Id,
            UserName = model.UserName,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            IsActive = (model.StatusCd == 'A')
         };
      }

      private CreateUserViewModel CreateNewCreateViewModel()
      {
         return new CreateUserViewModel()
         {
            UserName = "ABC",
            FirstName = "Abe",
            MiddleName = "Bobby",
            LastName = "Crabby",
            IsActive = true,
            NewPassword = "apassword"
         };
      }


      [ClassInitialize]
      public static void InitializeClass( TestContext context )
      {
         MapperConfig.RegisterMappings();
         Database.Initialize();
      }


      [TestInitialize]
      public virtual void InitializeTest()
      {
         SetupSessionFactory();
         SetupDatabase();
         SetupServices();
         SetupLogger();
         SetupControllerContext();

         CreateController();
      }

      private void SetupServices()
      {
         _securityService = new Mock<ISecurityService>();
      }

      private void SetupSessionFactory()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      private void SetupDatabase()
      {
         Database.Build( _session );
         Users.Load( _sessionFactory.Object );
      }

      private void CreateController()
      {
         _controller = new UsersController( _logger.Object, _securityService.Object, _sessionFactory.Object );
         _controller.ControllerContext = _controllerContext.Object;
      }

      private void SetupLogger()
      {
         _logger = new Mock<ILogger>();
      }

      private void SetupControllerContext()
      {
         _controllerContext = new Mock<ControllerContext>();
         _controllerContext
            .SetupSet( x => x.HttpContext.Session["NavigationStack"] = It.IsAny<Stack<NavigationData>>() )
            .Callback( ( string name, object m ) => { _navigationStack = (Stack<NavigationData>)m; } );
         _controllerContext
            .Setup( x => x.HttpContext.Session["NavigationStack"] )
            .Returns( () => _navigationStack );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }
      #endregion


      #region Index
      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( IEnumerable<UserViewModel> ) );
         Assert.AreEqual( Users.ModelData.Count(), ((IEnumerable<UserViewModel>)view.Model).Count() );
      }
      #endregion


      #region Create GET
      [TestMethod]
      public void CreateGet_ReturnsViewWithViewModel()
      {
         var result = _controller.Create() as PartialViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( CreateUserViewModel ) );
      }

      [TestMethod]
      public void CreateGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var viewModel = ((PartialViewResult)_controller.Create()).Model as CreateUserViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_AddsCallingActionAndId_IfSpecified()
      {
         var parentId = Guid.NewGuid();

         var viewModel = ((PartialViewResult)_controller.Create( "Edit", parentId.ToString() )).Model as CreateUserViewModel;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_SetsEditModeToCreate()
      {
         var parentId = Guid.NewGuid();
         var result = _controller.Create() as ViewResult;
         var viewModel = ((PartialViewResult)_controller.Create( "Edit", parentId.ToString() )).Model as CreateUserViewModel;

         Assert.AreEqual( EditMode.Create, viewModel.Mode );
      }

      [TestMethod]
      public void CreateGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Create( callingAction: "Index" );
         var viewModel = ((PartialViewResult)_controller.Create( callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Create( callingAction: "Index" );
         _controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Create( callingAction: "Index" );
         _controller.Create( callingAction: "Index" );
         _controller.Create( callingAction: "Run" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Run", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void CreateGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Create( callingAction: "Index" );
         _controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((PartialViewResult)_controller.Create()).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }
      #endregion


      #region Create POST
      [TestMethod]
      public void CreatePost_SavesUserIfModelIsValid()
      {
         var viewModel = CreateNewCreateViewModel();

         var result = _controller.Create( viewModel );

         _session.Clear();
         var items = _session.Query<User>()
            .Where( x => x.UserName == viewModel.UserName )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( viewModel.FirstName, items[0].FirstName );
      }

      [TestMethod]
      public void CreatePost_RedirectsToEditorModeReadOnly_IfModelIsValid()
      {
         var viewModel = CreateNewCreateViewModel();

         var result = _controller.Create( viewModel ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotSaveUserIfModelIsNotValid()
      {
         var viewModel = CreateNewCreateViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel );

         _session.Clear();
         var items = _session.Query<User>()
            .Where( x => x.UserName == viewModel.UserName )
            .ToList();

         Assert.AreEqual( 0, items.Count );
      }

      [TestMethod]
      public void CreatePost_ReturnsToEditorModeCreate_IfModelIsNotValid()
      {
         var viewModel = CreateNewCreateViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel ) as ViewResult;
         var vm = result.Model as UserEditorViewModel;

         Assert.AreEqual( Guid.Empty, vm.Id );
         Assert.AreEqual( EditMode.Create, vm.Mode );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidationFails()
      {
         var model = new CreateUserViewModel()
         {
            Id = Guid.NewGuid(),
            FirstName = "Bob",
            MiddleName = "The",
            LastName = "Builder",
            UserName = "BTB",
            NewPassword = "bogus",
            ConfirmPassword = "bogus"
         };

         model.FirstName = "";
         var result = _controller.Create( model );

         Assert.AreEqual( 1, _controller.ModelState.Count );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var model = new CreateUserViewModel()
         {
            Id = Guid.NewGuid(),
            FirstName = "Bob",
            MiddleName = "The",
            LastName = "Builder",
            UserName = "BTB",
            NewPassword = "bogus",
            ConfirmPassword = "bogus"
         };

         var result = _controller.Create( model );

         Assert.AreEqual( 0, _controller.ModelState.Count );
      }
      #endregion


      #region Details GET
      [TestMethod]
      public void DetailsGet_ReturnsViewWithViewModel()
      {
         var user = Users.ModelData.ToArray()[3];

         var result = _controller.Details( user.Id ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNotNull( result.Model );
         Assert.IsInstanceOfType( result.Model, typeof( UserViewModel ) );
         Assert.AreEqual( user.Id, ((UserViewModel)result.Model).Id );
         Assert.AreEqual( user.FirstName, ((UserViewModel)result.Model).FirstName );
         Assert.AreEqual( user.LastName, ((UserViewModel)result.Model).LastName );
      }

      [TestMethod]
      public void DetailsGet_ReturnsNoDataFoundIfModelNotFound()
      {
         var result = _controller.Details( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void DetailsGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var id = Users.ModelData[3].Id;
         var viewModel = ((ViewResult)_controller.Details( id )).Model as ViewModelBase;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_AddsCallingActionAndId_IfSpecified()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)_controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, callingAction: "Index" );
         var viewModel = ((ViewResult)_controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, callingAction: "Index" );
         _controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Details( id, callingAction: "Index" );
         _controller.Details( id, callingAction: "Index" );
         _controller.Details( id, callingAction: "Run" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Run", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void DetailsGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Details( id, callingAction: "Index" );
         _controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((ViewResult)_controller.Details( id )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }
      #endregion


      #region Edit GET
      [TestMethod]
      public void EditGet_ReturnsViewWithEditorModel()
      {
         var user = Users.ModelData.ToArray()[3];

         var result = _controller.Edit( user.Id ) as PartialViewResult;
         Assert.IsNotNull( result );

         Assert.IsInstanceOfType( result.Model, typeof( EditUserViewModel ) );
         Assert.AreEqual( user.Id, ((EditUserViewModel)result.Model).Id );
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfUserNotFound()
      {
         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var id = Users.ModelData.ToArray()[0].Id;

         var result = _controller.Edit( id ) as PartialViewResult;
         var viewModel = result.Model as EditUserViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_SetsCallingActionAndId_IfSupplied()
      {
         var id = Users.ModelData.ToArray()[0].Id;
         var callingId = Guid.NewGuid();

         var result = _controller.Edit( id, "Details", callingId.ToString() ) as PartialViewResult;
         var viewModel = result.Model as EditUserViewModel;

         Assert.AreEqual( viewModel.CallingAction, "Details" );
         Assert.AreEqual( callingId, viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Edit( id, callingAction: "Index" );
         var viewModel = ((PartialViewResult)_controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_DoesNotPush_IfCallingDataAlreadyOnTop()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Edit( id, callingAction: "Index" );
         _controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         _controller.Edit( id, callingAction: "Index" );
         _controller.Edit( id, callingAction: "Index" );
         _controller.Edit( id, callingAction: "Run" );

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Run", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Edit", navData.Action );
         Assert.AreEqual( parentId, new Guid( navData.Id ) );

         navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );
      }

      [TestMethod]
      public void EditGet_PopsFromNavigationStack_IfCallingDataNotGiven()
      {
         var id = Users.ModelData[3].Id;
         var parentId = Guid.NewGuid();

         _controller.Edit( id, callingAction: "Index" );
         _controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((PartialViewResult)_controller.Edit( id )).Model as ViewModelBase;

         var stack = _controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 1, stack.Count );

         var navData = stack.Peek();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         Assert.AreEqual( "Index", viewModel.CallingAction );
         Assert.AreEqual( Guid.Empty, viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_SetsEditModeToEdit()
      {
         var user = Users.ModelData.ToArray()[3];

         var result = _controller.Edit( user.Id ) as PartialViewResult;
         var viewModel = result.Model as EditUserViewModel;

         Assert.AreEqual( EditMode.Edit, viewModel.Mode );
      }
      #endregion


      #region Edit POST
      [TestMethod]
      public void EditPost_UpdatesUserIfModelValid()
      {
         var user = Users.ModelData.ToArray()[2];
         var model = new EditUserViewModel()
         {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            IsActive = (user.StatusCd == 'A')
         };

         model.FirstName += " Modified";
         _controller.Edit( model );

         _session.Clear();
         var item = _session.Get<User>( model.Id );
         Assert.AreEqual( model.FirstName, item.FirstName );
      }

      [TestMethod]
      public void EditPost_DoesNotSaveModelIfModelIsNotValid()
      {
         var user = Users.ModelData.ToArray()[2];
         var model = new EditUserViewModel()
         {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            IsActive = (user.StatusCd == 'A')
         };

         var origFirstName = model.FirstName;
         model.FirstName += " Modified";
         _controller.ModelState.AddModelError( "Test", "This is an error" );
         _controller.Edit( model );

         _session.Clear();
         var item = _session.Get<User>( model.Id );
         Assert.AreEqual( origFirstName, item.FirstName );
         Assert.AreNotEqual( model.FirstName, item.FirstName );
      }

      [TestMethod]
      public void EditPost_RedirectsToEditorModeReadOnly_IfModelIsValid()
      {
         var user = Users.ModelData.ToArray()[2];
         var model = new EditUserViewModel()
         {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            IsActive = (user.StatusCd == 'A')
         };

         var result = _controller.Edit( model ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsToEditorModeEdit_IfModelIsNotValid()
      {
         var user = Users.ModelData.ToArray()[2];
         var model = new EditUserViewModel()
         {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            IsActive = (user.StatusCd == 'A')
         };

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( model ) as ViewResult;
         var vm = result.Model as UserEditorViewModel;

         Assert.AreEqual( model.Id, vm.Id );
         Assert.AreEqual( EditMode.Edit, vm.Mode );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidationFails()
      {
         var user = Users.ModelData.ToArray()[3];
         var model = new EditUserViewModel()
         {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            IsActive = (user.StatusCd == 'A')
         };

         model.FirstName = "";
         var result = _controller.Edit( model );

         Assert.AreEqual( 1, _controller.ModelState.Count );
         Assert.IsTrue( _controller.ModelState.ContainsKey( "FirstName" ) );
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var user = Users.ModelData.ToArray()[3];
         var model = new EditUserViewModel()
         {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName,
            IsActive = (user.StatusCd == 'A')
         };

         var result = _controller.Edit( model );

         Assert.AreEqual( 0, _controller.ModelState.Count );
      }

      [TestMethod]
      public void EditPost_DoesNotCallsSecurityServiceToSetPassword()
      {
         var model = CreateNewEditViewModel( Users.ModelData[0] );
         model.NewPassword = "something";
         model.ConfirmPassword = model.NewPassword;

         _controller.Edit( model );

         _securityService
            .Verify( x => x.ChangePassword( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() ), Times.Never() );
      }
      #endregion
   }
}
