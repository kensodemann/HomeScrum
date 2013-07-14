using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Ninject.Extensions.Logging;
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
      protected Mock<IValidator<ModelT>> _validator;
      protected Mock<ILogger> _logger;
      protected IPrincipal FakeUser = new GenericPrincipal( new GenericIdentity( "ken", "Forms" ), null );
      protected Mock<ISessionFactory> _sessionFactory;
      protected Mock<ISession> _session;
      protected Mock<ITransaction> _transaction;

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

      public virtual void InitializeTest()
      {
         SetupSessionFactory();
         _validator = new Mock<IValidator<ModelT>>();
         _logger = new Mock<ILogger>();

         _validator.Setup( x => x.ModelIsValid( It.IsAny<ModelT>(), It.IsAny<TransactionType>() ) ).Returns( true );
      }

      public abstract ReadWriteController<ModelT, ViewModelT, EditorViewModelT> CreateDatabaseConnectedController();
      public abstract ReadWriteController<ModelT, ViewModelT, EditorViewModelT> CreateDatabaseMockedController();
      #endregion


      [TestMethod]
      public void Index_ReturnsViewWithAllItem()
      {
         var controller = CreateDatabaseConnectedController();

         var view = controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<SystemDomainObjectViewModel>;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.AreEqual( GetAllModels().Count, model.Count() );
      }


      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var controller = CreateDatabaseMockedController();
         var model = GetAllModels().ToArray()[2];

         _session.Setup( x => x.Get<ModelT>( model.Id ) )
            .Returns( model );

         var view = controller.Details( model.Id ) as ViewResult;

         _session.Verify( x => x.Get<ModelT>( model.Id ), Times.Once() );

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
         var controller = CreateDatabaseMockedController();
         var id = Guid.NewGuid();

         _session.Setup( x => x.Get<ModelT>( id ) ).Returns( null as ModelT );

         var result = controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreateGet_ReturnsViewWithViewModel()
      {
         var controller = CreateDatabaseMockedController();

         var result = controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result.Model, typeof( EditorViewModelT ) );
      }

      [TestMethod]
      public void CreatePost_CallsSaveAndCommitIfNewViewModelIsValid()
      {
         var controller = CreateDatabaseMockedController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser );

         _session.Verify( x => x.BeginTransaction(), Times.Once() );
         _transaction.Verify( x => x.Commit(), Times.Once() );
         _session.Verify( x => x.Save( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateDatabaseMockedController();
         var viewModel = CreateEditorViewModel();

         var result = controller.Create( viewModel, FakeUser ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotSaveOrCommitIfModelIsNotValid()
      {
         var controller = CreateDatabaseMockedController();

         var model = CreateEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, FakeUser );

         _session.Verify( x => x.BeginTransaction(), Times.Never() );
         _transaction.Verify( x => x.Commit(), Times.Never() );
         _session.Verify( x => x.Save( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = CreateEditorViewModel();

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Create( model, FakeUser );

         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result, typeof( ViewResult ) );
      }

      [TestMethod]
      public void CreatePost_PassesModelToValidator()
      {
         var controller = CreateDatabaseMockedController();
         var viewModel = CreateEditorViewModel();

         controller.Create( viewModel, FakeUser );

         _validator.Verify( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), TransactionType.Insert ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var viewModel = CreateEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = controller.Create( viewModel, FakeUser );

         Assert.AreEqual( messages.Count, controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var viewModel = CreateEditorViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = controller.Create( viewModel, FakeUser );

         Assert.AreEqual( 0, controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditGet_CallsSessionGet()
      {
         var controller = CreateDatabaseMockedController();

         Guid id = Guid.NewGuid();
         controller.Edit( id );

         _session.Verify( x => x.Get<ModelT>( id ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithViewModel()
      {
         var controller = CreateDatabaseMockedController();
         var model = GetAllModels().ToArray()[3];
         _session.Setup( x => x.Get<ModelT>( model.Id ) ).Returns( model );

         var result = controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNotNull( result.Model );
         Assert.IsInstanceOfType( result.Model, typeof( EditorViewModelT ) );
         Assert.AreEqual( model.Id, ((EditorViewModelT)result.Model).Id );
         Assert.AreEqual( model.Name, ((EditorViewModelT)result.Model).Name );
         Assert.AreEqual( model.Description, ((EditorViewModelT)result.Model).Description );
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      {
         var controller = CreateDatabaseMockedController();
         _session.Setup( x => x.Get<ModelT>( It.IsAny<Guid>() ) ).Returns( null as ModelT );

         var result = controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_CallRepositoryUpdateIfModelValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         controller.Edit( viewModel, FakeUser );

         _session.Verify( x => x.BeginTransaction(), Times.Once() );
         _transaction.Verify( x => x.Commit(), Times.Once() );
         _session.Verify( x => x.Update( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ) ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      {
         var controller = CreateDatabaseMockedController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         controller.Edit( viewModel, FakeUser );

         _session.Verify( x => x.BeginTransaction(), Times.Never() );
         _transaction.Verify( x => x.Commit(), Times.Never() );
         _session.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var controller = CreateDatabaseMockedController();
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
         var controller = CreateDatabaseMockedController();
         var model = GetAllModels().ToArray()[2];
         var viewModel = CreateEditorViewModel( model );

         controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = controller.Edit( viewModel, FakeUser ) as ViewResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_PassesModelToValidator()
      {
         var controller = CreateDatabaseMockedController();
         var model = GetAllModels().ToArray()[3];
         var viewModel = CreateEditorViewModel( model );

         controller.Edit( viewModel, FakeUser );

         _validator.Verify( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), TransactionType.Update ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var model = GetAllModels().ToArray()[3];
         var viewModel = CreateEditorViewModel( model );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = controller.Edit( viewModel, FakeUser );

         Assert.AreEqual( messages.Count, controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var model = GetAllModels().ToArray()[3];
         var viewModel = CreateEditorViewModel( model );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = controller.Edit( viewModel, FakeUser );

         Assert.AreEqual( 0, controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsInstanceOfType( result, typeof( RedirectToRouteResult ) );
      }

      [TestMethod]
      public void EditPost_PerformsDomainValidations()
      {
         var controller = CreateDatabaseMockedController();
         var messages = CreateStockErrorMessages();
         var model = GetAllModels().ToArray()[3];
         var viewModel = CreateEditorViewModel( model );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         // Name is required in all Domain objects
         viewModel.Name = "";

         var result = controller.Edit( viewModel, FakeUser );

         Assert.AreEqual( 1, controller.ModelState.Count );
         Assert.IsInstanceOfType( result, typeof( ViewResult ) );
      }


      #region private helpers
      ICollection<KeyValuePair<string, string>> CreateStockErrorMessages()
      {
         var messages = new List<KeyValuePair<string, string>>();

         messages.Add( new KeyValuePair<string, string>( "Name", "Name is not unique" ) );
         messages.Add( new KeyValuePair<string, string>( "SomethingElse", "Another Message" ) );

         return messages;
      }

      private void SetupSessionFactory()
      {
         _sessionFactory = new Mock<ISessionFactory>();
         _session = new Mock<ISession>();
         var _query = new Mock<ICriteria>();
         _transaction = new Mock<ITransaction>();

         _sessionFactory
            .Setup( x => x.OpenSession() )
            .Returns( _session.Object );

         _session
            .Setup( x => x.CreateCriteria( It.IsAny<Type>() ) )
            .Returns( _query.Object );
         _query
            .Setup( x => x.SetProjection( It.IsAny<ProjectionList>() ) )
            .Returns( _query.Object );
         _query
            .Setup( x => x.SetResultTransformer( It.IsAny<IResultTransformer>() ) )
            .Returns( _query.Object );

         _session
            .Setup( x => x.BeginTransaction() )
            .Returns( _transaction.Object );
      }
      #endregion
   }
}
