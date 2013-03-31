using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using System.Web.Mvc;
using HomeScrum.Common.TestData;
using System.Collections.Generic;
using System.Linq;
using HomeScrum.Web.Models;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class UsersControllerTest
   {
      private Mock<IRepository<User, Guid>> _userRepository;
      private Mock<ISecurityRepository> _securityRepository;
      private Mock<IValidator<User>> _validator;
      private UsersController _controller;

      private EditUserViewModel CreateNewEditViewModel()
      {
         var user = new User()
         {
            Id = Guid.NewGuid(),
            UserName = "ABC",
            FirstName = "Abe",
            MiddleName = "Bobby",
            LastName = "Crabby",
            IsActive = true
         };

         return new EditUserViewModel( user );
      }

      private CreateUserViewModel CreateNewCreateViewModel()
      {
         return new CreateUserViewModel()
         {
            UserName = "ABC",
            FirstName = "Abe",
            MiddleName = "Bobby",
            LastName = "Crabby",
            IsActive = true
         };
      }


      [TestInitialize]
      public virtual void InitializeTest()
      {
         Users.CreateTestModelData();

         _userRepository = new Mock<IRepository<User, Guid>>();
         _securityRepository = new Mock<ISecurityRepository>();
         _validator = new Mock<IValidator<User>>();

         _validator.Setup( x => x.ModelIsValid( It.IsAny<User>(), It.IsAny<TransactionType>() ) ).Returns( true );

         _controller = new UsersController( _userRepository.Object, _securityRepository.Object, _validator.Object );
      }

      [TestMethod]
      public void Index_GetsAllItems()
      {
         _controller.Index();

         _userRepository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _userRepository.Setup( x => x.GetAll() )
            .Returns( Users.ModelData );

         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }


      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = Users.ModelData.ToArray()[2];

         _userRepository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var view = _controller.Details( model.Id ) as ViewResult;

         _userRepository.Verify( x => x.Get( model.Id ), Times.Once() );

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.AreEqual( model, view.Model );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         _userRepository.Setup( x => x.Get( id ) ).Returns( null as User );

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
      public void CreatePost_CallsRepositoryAddIfNewModelIsValid()
      {
         var model = CreateNewCreateViewModel();

         var result = _controller.Create( model );

         _userRepository.Verify( x => x.Add( It.Is<User>( user => user.UserName == model.UserName ) ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var model = CreateNewCreateViewModel();

         var result = _controller.Create( model ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void CreatePost_DoesNotCallRepositoryAddIfModelIsNotValid()
      {
         var model = CreateNewCreateViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model );

         _userRepository.Verify( x => x.Add( It.IsAny<User>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var model = CreateNewCreateViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model ) as ViewResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreatePost_PassesUserToValidator()
      {
         var model = new CreateUserViewModel();

         _controller.Create( model );

         _validator.Verify( x => x.ModelIsValid( model, TransactionType.Insert ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var model = new CreateUserViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( false );

         var result = _controller.Create( model );

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
         var model = new CreateUserViewModel();

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Create( model );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void CreatePost_CallsSecurityRepositoryToSetPassword()
      {
         var model = CreateNewCreateViewModel();
         model.NewPassword = "NewPassword";

         _controller.Create( model );

         _securityRepository
            .Verify( x => x.ChangePassword( model.UserName, "bogus", model.NewPassword ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_CallsRepositoryGet()
      {
         var id = Guid.NewGuid();
         _controller.Edit( id );

         _userRepository.Verify( x => x.Get( id ), Times.Once() );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithEditorModel()
      {
         var user = Users.ModelData.ToArray()[3];
         _userRepository.Setup( x => x.Get( user.Id ) ).Returns( user );

         var result = _controller.Edit( user.Id ) as ViewResult;
         Assert.IsNotNull( result );

         var editorModel = result.Model as UserEditorViewModel;
         Assert.IsNotNull( editorModel );
         Assert.AreEqual( user.Id, editorModel.Id );
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfUserNotFoundInRepository()
      {
         _userRepository.Setup( x => x.Get( It.IsAny<Guid>() ) ).Returns( null as User );

         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_CallRepositoryUpdateIfModelValid()
      {
         var model = new EditUserViewModel( Users.ModelData.ToArray()[2] );

         _controller.Edit( model );

         _userRepository.Verify( x => x.Update( It.Is<User>( user => user.UserName == model.UserName ) ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      {
         var model = new EditUserViewModel( Users.ModelData.ToArray()[2] );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         _controller.Edit( model );

         _userRepository.Verify( x => x.Update( It.IsAny<User>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var model = new EditUserViewModel( Users.ModelData.ToArray()[2] );

         var result = _controller.Edit( model ) as RedirectToRouteResult;

         Assert.IsNotNull( result );
         Assert.AreEqual( 1, result.RouteValues.Count );

         object value;
         result.RouteValues.TryGetValue( "action", out value );
         Assert.AreEqual( "Index", value.ToString() );
      }

      [TestMethod]
      public void EditPost_ReturnsViewIfModelIsNotValid()
      {
         var model = new EditUserViewModel( Users.ModelData.ToArray()[2] );

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( model ) as ViewResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_PassesUserToValidator()
      {
         var model = new EditUserViewModel( Users.ModelData.ToArray()[3] );

         _controller.Edit( model );

         _validator.Verify( x => x.ModelIsValid( model, TransactionType.Update ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      {
         var messages = CreateStockErrorMessages();
         var model = new EditUserViewModel( Users.ModelData.ToArray()[3] );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( false );

         var result = _controller.Edit( model );

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
         var model = new EditUserViewModel( Users.ModelData.ToArray()[3] );

         _validator.SetupGet( x => x.Messages ).Returns( messages );
         _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( true );

         var result = _controller.Edit( model );

         Assert.AreEqual( 0, _controller.ModelState.Count );
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditPost_DoesNotCallsSecurityRepositoryToSetPassword()
      {
         var model = CreateNewEditViewModel();
         model.NewPassword = "something";
         model.ConfirmPassword = model.NewPassword;

         _controller.Edit( model );

         _securityRepository
            .Verify( x => x.ChangePassword( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>() ), Times.Never() );
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
