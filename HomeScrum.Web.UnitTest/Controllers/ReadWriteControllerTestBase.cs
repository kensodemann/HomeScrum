using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Linq;
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
   public abstract class ReadWriteControllerTestBase<ModelT, ViewModelT, EditorViewModelT>
      where ModelT : DomainObjectBase, new()
      where ViewModelT : DomainObjectViewModel, new()
      where EditorViewModelT : DomainObjectViewModel, IEditorViewModel, new()
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;

      protected Mock<ILogger> _logger;
      protected IPrincipal FakeUser = new GenericPrincipal( new GenericIdentity( "ken", "Forms" ), null );

      protected Mock<ControllerContext> _controllerConext;
      protected Stack<NavigationData> _navigationStack;

      protected abstract ICollection<ModelT> GetAllModels();
      protected abstract ModelT CreateNewModel();

      protected ISession _session;
      protected Mock<ISessionFactory> _sessionFactory;

      protected virtual EditorViewModelT CreateEditorViewModel( ModelT model )
      {
         return new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };
      }

      protected virtual EditorViewModelT CreateEditorViewModel()
      {
         return new EditorViewModelT()
         {
            //Id = Guid.NewGuid(),
            Name = "New Item",
            Description = "A new description"
         };
      }

      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();

         IntializeMapper();
      }

      private void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( _sessionFactory.Object );
      }

      private static void IntializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      public virtual void InitializeTest()
      {
         BuildMocks();
         CreateMockIOCKernel();
         SetupNHibernateSession();
      }

      private void BuildMocks()
      {
         _logger = new Mock<ILogger>();

         _controllerConext = new Mock<ControllerContext>();
         _controllerConext
            .SetupSet( x => x.HttpContext.Session["NavigationStack"] = It.IsAny<Stack<NavigationData>>() )
            .Callback( ( string name, object m ) => { _navigationStack = (Stack<NavigationData>)m; } );
         _controllerConext
            .Setup( x => x.HttpContext.Session["NavigationStack"] )
            .Returns( () => _navigationStack );

         _sessionFactory = new Mock<ISessionFactory>();
      }

      private void SetupNHibernateSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      public abstract ReadWriteController<ModelT, ViewModelT, EditorViewModelT> CreateController();
      #endregion


      #region Index
      [TestMethod]
      public void Index_ReturnsViewWithAllItem()
      {
         var controller = CreateController();

         var view = controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<SystemDomainObjectViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.AreEqual( GetAllModels().Count, model.Count() );
      }
      #endregion


      #region Create GET
      [TestMethod]
      public void CreateGet_ReturnsViewWithViewModel()
      {
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( EditorViewModelT ) );
      }

      [TestMethod]
      public void CreateGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var controller = CreateController();

         var viewModel = ((ViewResult)controller.Create()).Model as ViewModelBase;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_AddsCallingActionAndId_IfSpecified()
      {
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)controller.Create( callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void CreateGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         controller.Create( callingAction: "Index" );
         var viewModel = ((ViewResult)controller.Create( callingController: "BlueJackets", callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "BlueJackets", navData.Controller );
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
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         controller.Create( callingAction: "Index" );
         controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         controller.Create( callingController: "Preds", callingAction: "Index" );
         controller.Create( callingController: "Preds", callingAction: "Index" );
         controller.Create( callingAction: "Index" );

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "Preds", navData.Controller );
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
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         controller.Create( callingAction: "Index" );
         controller.Create( callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((ViewResult)controller.Create()).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void CreateGet_SetsModeToCreate()
      {
         var controller = CreateController();
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)controller.Create( callingAction: "Edit", callingId: parentId.ToString() )).Model as IEditorViewModel;

         Assert.AreEqual( EditMode.Create, viewModel.Mode );
      }
      #endregion


      #region Create POST
      [TestMethod]
      public void CreatePost_SavesModelIfNewViewModelIsValid()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser );

         _session.Clear();
         var items = _session.Query<ModelT>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 1, items.Count );
         Assert.AreEqual( viewModel.Name, items[0].Name );
         Assert.AreEqual( viewModel.Description, items[0].Description );
      }

      [TestMethod]
      public void CreatePost_RedirectsToCaller_IfModelIsValid()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         var callingId = Guid.NewGuid();
         controller.Create( callingAction: "MyStuff", callingController: "Stuff", callingId: callingId.ToString() );
         var result = controller.Create( viewModel, FakeUser ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 3, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "MyStuff", value.ToString() );

         result.RouteValues.TryGetValue( "controller", out value );
         Assert.AreEqual( "Stuff", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Guid actualId;
         if (!Guid.TryParse( value.ToString(), out actualId ))
         {
            Assert.Fail( "id was not a GUID" );
         }
         Assert.AreEqual( callingId, actualId );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndex_IfModeIsValidAndNoCaller()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotSaveModelIfModelIsNotValid()
      {
         var controller = CreateController();

         var viewModel = CreateEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, FakeUser );

         _session.Clear();
         var items = _session.Query<ModelT>()
            .Where( x => x.Name == viewModel.Name )
            .ToList();

         Assert.AreEqual( 0, items.Count );
      }

      [TestMethod]
      public void CreatePost_ReturnsToEditorModeCreate_IfModelIsNotValid()
      {
         var controller = CreateController();
         var model = CreateEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, FakeUser ) as ViewResult;
         var vm = result.Model as EditorViewModelT;

         Assert.AreEqual( Guid.Empty, vm.Id );
         Assert.AreEqual( EditMode.Create, vm.Mode );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidationFails()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         viewModel.Name = "";
         var result = controller.Create( viewModel, FakeUser );

         Assert.AreEqual( 1, controller.ModelState.Count );
         Assert.IsTrue( controller.ModelState.ContainsKey( "Name" ) );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser );

         Assert.AreEqual( 0, controller.ModelState.Count );
      }
      #endregion


      #region Details GET
      [TestMethod]
      public void DetailsGet_ReturnsViewWithViewModel()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[3];

         var result = controller.Details( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNotNull( result.Model );
         Assert.IsInstanceOfType( result.Model, typeof( ViewModelT ) );
         Assert.AreEqual( model.Id, ((ViewModelT)result.Model).Id );
         Assert.AreEqual( model.Name, ((ViewModelT)result.Model).Name );
         Assert.AreEqual( model.Description, ((ViewModelT)result.Model).Description );
      }

      [TestMethod]
      public void DetailsGet_ReturnsNoDataFoundIfModelNotFound()
      {
         var controller = CreateController();

         var result = controller.Details( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void DetailsGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;

         var viewModel = ((ViewResult)controller.Details( id )).Model as ViewModelBase;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_AddsCallingActionAndId_IfSpecified()
      {
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void DetailsGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         controller.Details( id, callingAction: "Index" );
         var viewModel = ((ViewResult)controller.Details( id, callingController: "Icing", callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Icing", navData.Controller );
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
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         controller.Details( id, callingAction: "Index" );
         controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Details( id, callingController: "NationWide", callingAction: "Index" );
         controller.Details( id, callingController: "NationWide", callingAction: "Index" );
         controller.Details( id, callingAction: "Index" );

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "NationWide", navData.Controller );
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
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         controller.Details( id, callingAction: "Index" );
         controller.Details( id, callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((ViewResult)controller.Edit( id )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void EditGet_ReturnsViewWithViewModel()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[3];

         var result = controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNotNull( result.Model );
         Assert.IsInstanceOfType( result.Model, typeof( EditorViewModelT ) );
         Assert.AreEqual( model.Id, ((EditorViewModelT)result.Model).Id );
         Assert.AreEqual( model.Name, ((EditorViewModelT)result.Model).Name );
         Assert.AreEqual( model.Description, ((EditorViewModelT)result.Model).Description );
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFound()
      {
         var controller = CreateController();

         var result = controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditGet_LeavesCallingActionAndIdAsDefault_IfNotSupplied()
      {
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;

         var viewModel = ((ViewResult)controller.Edit( id )).Model as ViewModelBase;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_AddsCallingActionAndId_IfSpecified()
      {
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         Assert.AreEqual( "Edit", viewModel.CallingAction );
         Assert.AreEqual( parentId, viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_PushesToNavigationStack_IfCallingDataGiven()
      {
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         controller.Edit( id, callingAction: "Index" );
         var viewModel = ((ViewResult)controller.Edit( id, callingController: "Icing", callingAction: "Edit", callingId: parentId.ToString() )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 2, stack.Count );

         var navData = stack.Pop();
         Assert.AreEqual( "Icing", navData.Controller );
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
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         controller.Edit( id, callingAction: "Index" );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         controller.Edit( id, callingController: "NationWide", callingAction: "Index" );
         controller.Edit( id, callingController: "NationWide", callingAction: "Index" );
         controller.Edit( id, callingAction: "Index" );

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

         Assert.IsNotNull( stack );
         Assert.AreEqual( 4, stack.Count );

         var navData = stack.Pop();
         Assert.IsNull( navData.Controller );
         Assert.AreEqual( "Index", navData.Action );
         Assert.IsNull( navData.Id );

         navData = stack.Pop();
         Assert.AreEqual( "NationWide", navData.Controller );
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
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         controller.Edit( id, callingAction: "Index" );
         controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() );
         var viewModel = ((ViewResult)controller.Edit( id )).Model as ViewModelBase;

         var stack = controller.Session["NavigationStack"] as Stack<NavigationData>;

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
      public void EditGet_SetsModeToEdit()
      {
         var controller = CreateController();
         var id = GetAllModels().ToArray()[3].Id;
         var parentId = Guid.NewGuid();

         var viewModel = ((ViewResult)controller.Edit( id, callingAction: "Edit", callingId: parentId.ToString() )).Model as IEditorViewModel;

         Assert.AreEqual( EditMode.Edit, viewModel.Mode );
      }
      #endregion


      #region Edit POST
      [TestMethod]
      public void EditPost_SavesModelIfModelValid()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         viewModel.Name += " Modified";
         controller.Edit( viewModel, FakeUser );

         _session.Clear();
         var item = _session.Get<ModelT>( viewModel.Id );
         Assert.AreEqual( viewModel.Name, item.Name );
      }

      [TestMethod]
      public void EditPost_DoesNotSaveModelIfModelIsNotValid()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var origName = viewModel.Name;
         viewModel.Name += " Modified";
         controller.Edit( viewModel, FakeUser );

         _session.Clear();
         var item = _session.Get<ModelT>( viewModel.Id );
         Assert.AreNotEqual( viewModel.Name, item.Name );
         Assert.AreEqual( origName, item.Name );
      }

      [TestMethod]
      public void EditPost_RedirectsToCaller_IfModelIsValid()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         var callingId = Guid.NewGuid();
         controller.Edit( model.Id, callingAction: "Sleep", callingController: "Soma", callingId: callingId.ToString() );
         _session.Clear();
         var result = controller.Edit( viewModel, FakeUser ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 3, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Sleep", value.ToString() );

         result.RouteValues.TryGetValue( "controller", out value );
         Assert.AreEqual( "Soma", value.ToString() );

         result.RouteValues.TryGetValue( "id", out value );
         Guid actualId;
         if (!Guid.TryParse( value.ToString(), out actualId ))
         {
            Assert.Fail( "id was not a GUID" );
         }
         Assert.AreEqual( callingId, actualId );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndex_IfModelIsValidAndNoCaller()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         var result = controller.Edit( viewModel, FakeUser ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsToModelModeEdit_IfModelIsNotValid()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, FakeUser ) as ViewResult;
         var vm = result.Model as EditorViewModelT;

         Assert.AreEqual( model.Id, vm.Id );
         Assert.AreEqual( EditMode.Edit, vm.Mode );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidationFails()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[3];
         var viewModel = CreateEditorViewModel( model );

         viewModel.Name = "";
         var result = controller.Edit( viewModel, FakeUser );

         Assert.AreEqual( 1, controller.ModelState.Count );
         Assert.IsTrue( controller.ModelState.ContainsKey( "Name" ) );
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[3];
         var viewModel = CreateEditorViewModel( model );

         var result = controller.Edit( viewModel, FakeUser );

         Assert.AreEqual( 0, controller.ModelState.Count );
      }
      #endregion
   }
}
