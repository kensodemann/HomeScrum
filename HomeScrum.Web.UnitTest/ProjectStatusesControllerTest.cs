using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Controllers;
using HomeScrum.Common.TestData;
using System.Collections.Generic;
using System.Web.Mvc;


namespace HomeScrum.Web.UnitTest
{
   [TestClass]
   public class ProjectStatusesControllerTest
   {
      private Mock<IDataObjectRepository<ProjectStatus>> _repository;
      private ProjectStatusesController _controller;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<ProjectStatus>>();
         _controller = new ProjectStatusesController( _repository.Object );
      }


      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _repository.Setup( x => x.GetAll() )
            .Returns( ProjectStatuses.ModelData );

         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllProjectStatuses()
      {
         _controller.Index();

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = ProjectStatuses.ModelData[2];

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

         _repository.Setup( x => x.Get( id ) ).Returns( null as ProjectStatus );

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
      public void CreatePost_CallsRepositoryAddIfNoModelIsValid()
      {
         var model = new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            IsPredefined = 'N',
            IsActive = 'Y',
            StatusCd = 'A'
         };
         var result = _controller.Create( model );

         _repository.Verify( x => x.Add( model ), Times.Once() );
      }

      [TestMethod]
      public void CreatePost_RedirectsToIndexIfModelIsValid()
      {
         var model = new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            IsPredefined = 'N',
            IsActive = 'Y',
            StatusCd = 'A'
         };
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
         var model = new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            IsPredefined = 'N',
            IsActive = 'Y',
            StatusCd = 'A'
         };
         _controller.ModelState.AddModelError( "ProjectStatus", "This is an error" );
         var result = _controller.Create( model );

         _repository.Verify( x => x.Add( It.IsAny<ProjectStatus>() ), Times.Never() );
      }

      [TestMethod]
      public void CreatePost_ReturnsViewIfModelIsNotValid()
      {
         var model = new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            IsPredefined = 'N',
            IsActive = 'Y',
            StatusCd = 'A'
         };
         _controller.ModelState.AddModelError( "ProjectStatus", "This is an error" );
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
   }
}
