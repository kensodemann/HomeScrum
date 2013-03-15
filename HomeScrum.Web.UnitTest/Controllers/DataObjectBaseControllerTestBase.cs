using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Controllers.Base;
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
   public abstract class DataObjectBaseControllerTestBase<T> where T : DataObjectBase
   {
      protected Mock<IDataObjectRepository<T>> _repository;
      protected Mock<IValidator<T>> _validator;
      protected DataObjectBaseController<T> _controller;

      protected abstract ICollection<T> GetAllModels();
      protected abstract T CreateNewModel();


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

         _repository.Setup( x => x.Get( id ) ).Returns( null as T );

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
         var model = CreateNewModel();

         var result = _controller.Create( model );

         _repository.Verify( x => x.Add( model ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var model = CreateNewModel();

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
         var model = CreateNewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model );

         _repository.Verify( x => x.Add( It.IsAny<T>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var model = CreateNewModel();

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Create( model ) as ViewResult;

         Assert.IsNotNull( result );
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
         Assert.AreEqual( model, result.Model );
      }

      [TestMethod]
      public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      {
         _repository.Setup( x => x.Get( It.IsAny<Guid>() ) ).Returns( null as T );

         var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void EditPost_CallRepositoryUpdateIfModelValid()
      {
         var model = GetAllModels().ToArray()[2];

         _controller.Edit( model );

         _repository.Verify( x => x.Update( model ), Times.Once() );
      }

      [TestMethod]
      public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      {
         var model = GetAllModels().ToArray()[2];

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         _controller.Edit( model );

         _repository.Verify( x => x.Update( It.IsAny<T>() ), Times.Never() );
      }

      [TestMethod]
      public void EditPost_RedirectsToIndexIfModelIsValid()
      {
         var model = GetAllModels().ToArray()[2];

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
         var model = GetAllModels().ToArray()[2];

         _controller.ModelState.AddModelError( "Test", "This is an error" );
         var result = _controller.Edit( model ) as ViewResult;

         Assert.IsNotNull( result );
      }
   }
}
