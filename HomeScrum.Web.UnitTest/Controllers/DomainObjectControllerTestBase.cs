using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   public abstract class DomainObjectControllerTestBase<ModelT, EditViewModelT>
      where ModelT : DataObjectBase, new()
      where EditViewModelT : IViewModel<ModelT>, new()
   {
      protected Mock<IRepository<ModelT, Guid>> _repository;
      protected Mock<IValidator<ModelT>> _validator;
      protected DomainObjectController<ModelT, EditViewModelT> _controller;

      protected abstract ICollection<ModelT> GetAllModels();
      protected abstract ModelT CreateNewModel();

      public virtual void InitializeTest()
      {
         _repository = new Mock<IRepository<ModelT, Guid>>();
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
         Assert.AreEqual( model, view.Model );
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
      public void CreateGet_ReturnsViewWithViewModelEmptyDomainModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNotNull( result.Model );

         var domainModel = ((EditViewModelT)result.Model).DomainModel;
         Assert.IsNotNull( domainModel );
         Assert.AreEqual( default( Guid ), domainModel.Id );
         Assert.IsNull( domainModel.Name );
         Assert.IsNull( domainModel.Description );
      }

      [TestMethod]
      public void CreatePost_CallsRepositoryAddIfNewModelIsValid()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = CreateNewModel()
         };

         var result = _controller.Create( viewModel );

         _repository.Verify( x => x.Add( viewModel.DomainModel ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = CreateNewModel()
         };

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
         var viewModel = new EditViewModelT()
         {
            DomainModel = CreateNewModel()
         };

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel );

         _repository.Verify( x => x.Add( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = CreateNewModel()
         };

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel ) as ViewResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreatePost_PassesModelToValidator()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[3]
         };

         _controller.Create( viewModel );

         _validator.Verify( x => x.ModelIsValid( viewModel.DomainModel, TransactionType.All ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[3]
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( viewModel.DomainModel, TransactionType.All ) ).Returns( false );

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
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[3]
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( viewModel.DomainModel, TransactionType.All ) ).Returns( true );

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
      public void EditGet_ReturnsViewWithModel()
      {
         var model = GetAllModels().ToArray()[3];
         _repository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNotNull( result.Model );
         Assert.AreEqual( model, ((EditViewModelT)result.Model).DomainModel );
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
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[2]
         };

         _controller.Edit( viewModel );

         _repository.Verify( x => x.Update( viewModel.DomainModel ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[2]
         };

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         _controller.Edit( viewModel );

         _repository.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[2]
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
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[2]
         };

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( viewModel ) as ViewResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_PassesModelToValidator()
      {
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[3]
         };

         _controller.Edit( viewModel );

         _validator.Verify( x => x.ModelIsValid( viewModel.DomainModel, TransactionType.All ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[3]
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( viewModel.DomainModel, TransactionType.All ) ).Returns( false );

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
         var viewModel = new EditViewModelT()
         {
            DomainModel = GetAllModels().ToArray()[3]
         };

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( viewModel.DomainModel, TransactionType.All ) ).Returns( true );

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
