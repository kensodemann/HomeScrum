using AutoMapper;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Linq;
using Ninject.Extensions.Logging;
using Ninject;
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
      where EditorViewModelT : DomainObjectViewModel, new()
   {
      #region Test Setup
      private static MoqMockingKernel _iocKernel;

      protected Mock<ILogger> _logger;
      protected IPrincipal FakeUser = new GenericPrincipal( new GenericIdentity( "ken", "Forms" ), null );

      protected abstract ICollection<ModelT> GetAllModels();
      protected abstract ModelT CreateNewModel();

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
            Id = Guid.NewGuid(),
            Name = "New Item",
            Description = "A new description"
         };
      }

      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();

         CreateMockIOCKernel();
         IntializeMapper();
      }

      private static void CreateMockIOCKernel()
      {
         _iocKernel = new MoqMockingKernel();
         _iocKernel.Bind<ISessionFactory>().ToConstant( Database.SessionFactory );
      }

      private static void IntializeMapper()
      {
         Mapper.Initialize( map => map.ConstructServicesUsing( x => _iocKernel.Get( x ) ) );
         MapperConfig.RegisterMappings();
      }

      public virtual void InitializeTest()
      {
         BuildMocks();
      }

      private void BuildMocks()
      {
         _logger = new Mock<ILogger>();
      }

      public abstract ReadWriteController<ModelT, ViewModelT, EditorViewModelT> CreateController();
      #endregion


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


      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];

         var view = controller.Details( model.Id ) as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( ViewModelT ) );
         Assert.AreEqual( model.Id, ((ViewModelT)view.Model).Id );
         Assert.AreEqual( model.Name, ((ViewModelT)view.Model).Name );
         Assert.AreEqual( model.Description, ((ViewModelT)view.Model).Description );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var controller = CreateController();
         var id = Guid.NewGuid();

         var result = controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreateGet_ReturnsViewWithViewModel()
      {
         var controller = CreateController();

         var result = controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( EditorViewModelT ) );
      }

      [TestMethod]
      public void CreatePost_SavesModelIfNewViewModelIsValid()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<ModelT>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( viewModel.Name, items[0].Name );
            Assert.AreEqual( viewModel.Description, items[0].Description );
         }
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotSaveModelIfModelIsNotValid()
      {
         var controller = CreateController();

         var viewModel = CreateEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( viewModel, FakeUser );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<ModelT>()
               .Where( x => x.Name == viewModel.Name )
               .ToList();

            Assert.AreEqual( 0, items.Count );
         }
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var controller = CreateController();
         var model = CreateEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, FakeUser );

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result, typeof( ViewResult ) );
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
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidationSucceeds()
      {
         var controller = CreateController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser );

         Assert.AreEqual( 0, controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

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
      public void EditPost_SavesModelIfModelValid()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         viewModel.Name += " Modified";
         controller.Edit( viewModel, FakeUser );

         using (var session = Database.OpenSession())
         {
            var item = session.Get<ModelT>( viewModel.Id );
            Assert.AreEqual( viewModel.Name, item.Name );
         }
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

         using (var session = Database.OpenSession())
         {
            var item = session.Get<ModelT>( viewModel.Id );
            Assert.AreNotEqual( viewModel.Name, item.Name );
            Assert.AreEqual( origName, item.Name );
         }
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         var result = controller.Edit( viewModel, FakeUser ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsViewIfModelIsNotValid()
      {
         var controller = CreateController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, FakeUser ) as ViewResult;

         Assert.IsNotNull( result );
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
         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result, typeof( RedirectToRouteResult ) );
      }
   }
}
