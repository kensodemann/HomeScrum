using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   public abstract class ReadWriteControllerTestBase<ModelT, ViewModelT, EditorViewModelT>
      where ModelT : DomainObjectBase, new()
      where ViewModelT : DomainObjectViewModel, new()
      where EditorViewModelT : DomainObjectEditorViewModel, new()
   {
      protected Mock<IRepository<ModelT>> _repository;
      protected Mock<IValidator<ModelT>> _validator;
      protected ReadWriteController<ModelT, ViewModelT, EditorViewModelT> _controller;

      protected abstract ICollection<ModelT> GetAllModels();
      protected abstract ModelT CreateNewModel();

      public virtual void InitializeTest()
      {
         _repository = new Mock<IRepository<ModelT>>();
         _validator = new Mock<IValidator<ModelT>>();

         _validator.Setup( x => x.ModelIsValid( It.IsAny<ModelT>(), It.IsAny<TransactionType>() ) ).Returns( true );
      }


      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _repository.Setup( x => x.GetAll() )
            .Returns( GetAllModels() );

         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( IEnumerable<ViewModelT> ) );
      }

      [TestMethod]
      public void Index_GetsAllItems()
      {
         _controller.Index();

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = GetAllModels().ToArray()[2];

         _repository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var view = _controller.Details( model.Id ) as ViewResult;

         _repository.Verify( x => x.Get( model.Id ), Times.Once() );

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
         var id = Guid.NewGuid();

         _repository.Setup( x => x.Get( id ) ).Returns( null as ModelT );

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreateGet_ReturnsViewWithoutModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNull( result.Model );
      }

      [TestMethod]
      public void CreatePost_CallsRepositoryAddIfNewViewModelIsValid()
      {
         var viewModel = new EditorViewModelT()
         {
            Id = Guid.NewGuid(),
            Name = "New Item",
            Description = "A new description"
         };

         var result = _controller.Create( viewModel );

         _repository.Verify( x => x.Add( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var viewModel = new EditorViewModelT();

         var result = _controller.Create( viewModel ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotCallRepositoryAddIfModelIsNotValid()
      {
         var model = new EditorViewModelT();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model );

         _repository.Verify( x => x.Add( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var model = new EditorViewModelT();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model ) as ViewResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreatePost_PassesModelToValidator()
      {
         var viewModel = new EditorViewModelT()
         {
            Id = Guid.NewGuid(),
            Name = "New Item",
            Description = "A new description"
         };

         _controller.Create( viewModel );

         _validator.Verify( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), TransactionType.Insert ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var viewModel = new EditorViewModelT()
         {
            Id = Guid.NewGuid(),
            Name = "New Item",
            Description = "A new description"
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = _controller.Create( viewModel );

         Assert.AreEqual( messages.Count, _controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( _controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void CreatePost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var messages = CreateStockErrorMessages();
         var viewModel = new EditorViewModelT()
         {
            Id = Guid.NewGuid(),
            Name = "New Item",
            Description = "A new description"
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Create( viewModel );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditGet_CallsRepositoryGet()
      {
         Guid id = Guid.NewGuid();
         _controller.Edit( id );

         _repository.Verify( x => x.Get( id ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithViewModel()
      {
         var model = GetAllModels().ToArray()[3];
         _repository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;

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
         _repository.Setup( x => x.Get( It.IsAny<Guid>() ) ).Returns( null as ModelT );

         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_CallRepositoryUpdateIfModelValid()
      {
         var model = GetAllModels().ToArray()[2];
         var viewModel = new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };

         _controller.Edit( viewModel );

         _repository.Verify( x => x.Update( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ) ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      {
         var model = GetAllModels().ToArray()[2];
         var viewModel = new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         _controller.Edit( viewModel );

         _repository.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var model = GetAllModels().ToArray()[2];
         var viewModel = new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };

         var result = _controller.Edit( viewModel ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsViewIfModelIsNotValid()
      {
         var model = GetAllModels().ToArray()[2];
         var viewModel = new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel ) as ViewResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_PassesModelToValidator()
      {
         var model = GetAllModels().ToArray()[3];
         var viewModel = new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };

         _controller.Edit( viewModel );

         _validator.Verify( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), TransactionType.Update ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var model = GetAllModels().ToArray()[3];
         var viewModel = new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( false );

         var result = _controller.Edit( viewModel );

         Assert.AreEqual( messages.Count, _controller.ModelState.Count );
         foreach (var message in messages)
         {
            Assert.IsTrue( _controller.ModelState.ContainsKey( message.Key ) );
         }
         Assert.IsTrue( result is ViewResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      {
         var messages = CreateStockErrorMessages();
         var model = GetAllModels().ToArray()[3];
         var viewModel = new EditorViewModelT()
         {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( It.Is<ModelT>( m => m.Id == viewModel.Id && m.Name == viewModel.Name && m.Description == viewModel.Description ), It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Edit( viewModel );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }


      #region private helpers
      ICollection<KeyValuePair<string, string>> CreateStockErrorMessages()
      {
         var messages = new List<KeyValuePair<string, string>>();

         messages.Add( new KeyValuePair<string, string>( "Name", "Name is not unique" ) );
         messages.Add( new KeyValuePair<string, string>( "SomethingElse", "Another Message" ) );

         return messages;
      }
      #endregion
   }
}
