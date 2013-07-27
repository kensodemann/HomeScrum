using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate.Context;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class UsersControllerTest
   {
      private Mock<ISecurityService> _securityService;
      private UsersController _controller;

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
         CurrentSessionContext.Bind( Database.SessionFactory.OpenSession() );

         Database.Build();
         Users.Load();

         _securityService = new Mock<ISecurityService>();

         _controller = new UsersController( _securityService.Object, Database.SessionFactory );
         _controller.ControllerContext = new ControllerContext();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         var session = CurrentSessionContext.Unbind( Database.SessionFactory );
         session.Dispose();
      }


      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( IEnumerable<UserViewModel> ) );
         Assert.AreEqual( Users.ModelData.Count(), ((IEnumerable<UserViewModel>)view.Model).Count() );
      }


      [TestMethod]
      public void Details_ReturnsViewWithViewModel()
      {
         var model = Users.ModelData.ToArray()[2];

         var view = _controller.Details( model.Id ) as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         Assert.IsInstanceOfType( view.Model, typeof( UserViewModel ) );
         Assert.AreEqual( model.Id, ((UserViewModel)view.Model).Id );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

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
      public void CreatePost_SavesUserIfModelIsValid()
      {
         var viewModel = CreateNewCreateViewModel();

         var result = _controller.Create( viewModel );

         using (var session = Database.OpenSession())
         {
            var items = session.Query<User>()
               .Where( x => x.UserName == viewModel.UserName )
               .ToList();

            Assert.AreEqual( 1, items.Count );
            Assert.AreEqual( viewModel.FirstName, items[0].FirstName );
         }
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
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

         using (var session = Database.OpenSession())
         {
            var items = session.Query<User>()
               .Where( x => x.UserName == viewModel.UserName )
               .ToList();

            Assert.AreEqual( 0, items.Count );
         }
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var viewModel = CreateNewCreateViewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( viewModel ) as ViewResult;

         Assert.IsNotNull( result );
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
         Assert.IsTrue( _controller.ModelState.ContainsKey( "FirstName" ) );
         Assert.IsTrue( result is ViewResult );
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
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
      }

      [TestMethod]
      public void EditGet_ReturnsViewWithEditorModel()
      {
         var user = Users.ModelData.ToArray()[3];

         var result = _controller.Edit( user.Id ) as ViewResult;
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

         var result = _controller.Edit( id ) as ViewResult;
         var viewModel = result.Model as EditUserViewModel;

         Assert.IsNull( viewModel.CallingAction );
         Assert.AreEqual( default( Guid ), viewModel.CallingId );
      }

      [TestMethod]
      public void EditGet_SetsCallingActionAndId_IfSupplied()
      {
         var id = Users.ModelData.ToArray()[0].Id;
         var callingId = Guid.NewGuid();

         var result = _controller.Edit( id, "Details", callingId.ToString() ) as ViewResult;
         var viewModel = result.Model as EditUserViewModel;

         Assert.AreEqual( viewModel.CallingAction, "Details" );
         Assert.AreEqual( callingId, viewModel.CallingId );
      }

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

         using (var session = Database.OpenSession())
         {
            var item = session.Get<User>( model.Id );
            Assert.AreEqual( model.FirstName, item.FirstName );
         }
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

         using (var session = Database.OpenSession())
         {
            var item = session.Get<User>( model.Id );
            Assert.AreEqual( origFirstName, item.FirstName );
            Assert.AreNotEqual( model.FirstName, item.FirstName );
         }
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
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
      public void EditPost_ReturnsViewIfModelIsNotValid()
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

         Assert.IsNotNull( result );
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
         Assert.IsNotNull( result );
         Assert.IsTrue( result is RedirectToRouteResult );
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
   }
}
