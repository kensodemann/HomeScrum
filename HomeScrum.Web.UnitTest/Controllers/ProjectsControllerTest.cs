using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using System.Collections.Generic;
using HomeScrum.Common.TestData;
using HomeScrum.Web.Controllers;
using System.Web.Mvc;
using Moq;
using HomeScrum.Data.Validators;
using HomeScrum.Data.Repositories;
using HomeScrum.Web.Models;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class ProjectsControllerTest
   {
      private Mock<IRepository<Project>> _projectRepository;
      private Mock<IRepository<ProjectStatus>> _projectStatusRepository;
      private Mock<IValidator<Project>> _validator;
      private ProjectsController _controller;

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public virtual void InitializeTest()
      {
         InitializeTestData();
         CreateRepositories();
         CreateValidator();

         _controller = new ProjectsController( _projectRepository.Object, _projectStatusRepository.Object, _validator.Object );
      }

      #region private helpers
      private void CreateValidator()
      {
         _validator = new Mock<IValidator<Project>>();
         _validator.Setup( x => x.ModelIsValid( It.IsAny<Project>(), It.IsAny<TransactionType>() ) ).Returns( true );
      }

      private void CreateRepositories()
      {
         _projectRepository = new Mock<IRepository<Project>>();
         _projectStatusRepository = new Mock<IRepository<ProjectStatus>>();
         _projectStatusRepository.Setup( x => x.GetAll() ).Returns( ProjectStatuses.ModelData );
         _projectRepository.Setup( x => x.GetAll() ).Returns( Projects.ModelData );
      }

      private static void InitializeTestData()
      {
         Users.CreateTestModelData( initializeIds: true );
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         Projects.CreateTestModelData( initializeIds: true );
      }

      ICollection<KeyValuePair<string, string>> CreateStockErrorMessages()
      {
         var messages = new List<KeyValuePair<string, string>>();

         messages.Add( new KeyValuePair<string, string>( "Name", "Name is not unique" ) );
         messages.Add( new KeyValuePair<string, string>( "SomethingElse", "Another Message" ) );

         return messages;
      }
      #endregion

      [TestMethod]
      public void Index_ReturnsViewWithModel()
      {
         _projectRepository.Setup( x => x.GetAll() )
            .Returns( Projects.ModelData );

         var view = _controller.Index() as ViewResult;

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
      }

      [TestMethod]
      public void Index_GetsAllItems()
      {
         _controller.Index();

         _projectRepository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void Details_ReturnsViewWithModel()
      {
         var model = Projects.ModelData[2];

         _projectRepository.Setup( x => x.Get( model.Id ) )
            .Returns( model );

         var view = _controller.Details( model.Id ) as ViewResult;

         _projectRepository.Verify( x => x.Get( model.Id ), Times.Once() );

         Assert.IsNotNull( view );
         Assert.IsNotNull( view.Model );
         //Assert.AreEqual( model, view.Model );
      }

      [TestMethod]
      public void Details_ReturnsHttpNotFoundIfNoModel()
      {
         var id = Guid.NewGuid();

         _projectRepository.Setup( x => x.Get( id ) ).Returns( null as Project );

         var result = _controller.Details( id ) as HttpNotFoundResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void CreateGet_ReturnsViewWithViewWithModel()
      {
         var result = _controller.Create() as ViewResult;

         Assert.IsNotNull( result );
         var model = result.Model as ProjectEditorViewModel;
         Assert.IsNotNull( model );
      }

      [TestMethod]
      public void CreateGet_InitializesProjectStatusList_NothingSelected()
      {
         var result = _controller.Create() as ViewResult;

         var model = result.Model as ProjectEditorViewModel;

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.StatusCd == 'A' ), model.ProjectStatuses.Count() );
         foreach (var item in model.ProjectStatuses)
         {
            var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }

      //[TestMethod]
      //public void CreatePost_CallsRepositoryAddIfNewModelIsValid()
      //{
      //   var model = new ProjectEditorViewModel();

      //   var result = _controller.Create( model );

      //   _projectRepository.Verify( x => x.Add( It.Is<Project>( p => p.Id == model.Id ) ), Times.Once() );
      //}

      //[TestMethod]
      //public void CreatePost_RedirectsToIndexIfModelIsValid()
      //{
      //   var model = new ProjectEditorViewModel();

      //   var result = _controller.Create( model ) as RedirectToRouteResult;

      //   Assert.IsNotNull( result );
      //   Assert.AreEqual( 1, result.RouteValues.Count );

      //   object value;
      //   result.RouteValues.TryGetValue( "action", out value );
      //   Assert.AreEqual( "Index", value.ToString() );
      //}

      //[TestMethod]
      //public void CreatePost_DoesNotCallRepositoryAddIfModelIsNotValid()
      //{
      //   var model = new ProjectEditorViewModel();

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Create( model );

      //   _projectRepository.Verify( x => x.Add( It.IsAny<Project>() ), Times.Never() );
      //}

      //[TestMethod]
      //public void CreatePost_ReturnsViewIfModelIsNotValid()
      //{
      //   var model = new ProjectEditorViewModel();

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Create( model ) as ViewResult;

      //   Assert.IsNotNull( result );
      //   Assert.AreEqual( model, result.Model );
      //}

      //[TestMethod]
      //public void CreatePost_InitializesProjectStatusList_NothingSelected()
      //{
      //   var model = new ProjectEditorViewModel();

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Create( model ) as ViewResult;

      //   var returnedModel = result.Model as ProjectEditorViewModel;

      //   Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.AllowUse ), returnedModel.ProjectStatuses.Count() );
      //   foreach (var item in returnedModel.ProjectStatuses)
      //   {
      //      var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
      //      Assert.AreEqual( status.Name, item.Text );
      //      Assert.IsFalse( item.Selected );
      //   }
      //}

      //[TestMethod]
      //public void CreatePost_PassesModelToValidator()
      //{
      //   var model = new ProjectEditorViewModel();

      //   _controller.Create( model );

      //   _validator.Verify( x => x.ModelIsValid( model, TransactionType.Insert ), Times.Once() );
      //}

      //[TestMethod]
      //public void CreatePost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      //{
      //   var messages = CreateStockErrorMessages();
      //   var model = new ProjectEditorViewModel();

      //   _validator.SetupGet( x => x.Messages ).Returns( messages );
      //   _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( false );

      //   var result = _controller.Create( model );

      //   Assert.AreEqual( messages.Count, _controller.ModelState.Count );
      //   foreach (var message in messages)
      //   {
      //      Assert.IsTrue( _controller.ModelState.ContainsKey( message.Key ) );
      //   }
      //   Assert.IsTrue( result is ViewResult );
      //}

      //[TestMethod]
      //public void CreatePost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      //{
      //   var messages = CreateStockErrorMessages();
      //   var model = new ProjectEditorViewModel();

      //   _validator.SetupGet( x => x.Messages ).Returns( messages );
      //   _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( true );

      //   var result = _controller.Create( model );

      //   Assert.AreEqual( 0, _controller.ModelState.Count );
      //   Assert.IsNotNull( result );
      //   Assert.IsTrue( result is RedirectToRouteResult );
      //}

      //[TestMethod]
      //public void EditGet_CallsRepositoryGet()
      //{
      //   Guid id = Guid.NewGuid();
      //   _controller.Edit( id );

      //   _projectRepository.Verify( x => x.Get( id ), Times.Once() );
      //}

      //[TestMethod]
      //public void EditGet_ReturnsViewWithModel()
      //{
      //   var model = Projects.ModelData[3];
      //   _projectRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

      //   var result = _controller.Edit( model.Id ) as ViewResult;

      //   Assert.IsNotNull( result );
      //   var returnedModel = result.Model as ProjectEditorViewModel;
      //   Assert.IsNotNull( returnedModel );
      //   Assert.AreEqual( model.Id, returnedModel.Id );
      //}

      //[TestMethod]
      //public void EditGet_InitializesProjectStatuses_ProjectStatusSelected()
      //{
      //   var model = Projects.ModelData[0];
      //   _projectRepository.Setup( x => x.Get( model.Id ) ).Returns( model );

      //   var result = _controller.Edit( model.Id ) as ViewResult;
      //   var viewModel = result.Model as ProjectEditorViewModel;

      //   Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.AllowUse ), viewModel.ProjectStatuses.Count() );
      //   foreach (var item in viewModel.ProjectStatuses)
      //   {
      //      var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
      //      Assert.AreEqual( status.Name, item.Text );
      //      Assert.IsTrue( (model.ProjectStatus.Id.ToString() != item.Value && !item.Selected) ||
      //                     (model.ProjectStatus.Id.ToString() == item.Value && item.Selected) );
      //   }
      //}

      //[TestMethod]
      //public void EditGet_ReturnsNoDataFoundIfModelNotFoundInRepository()
      //{
      //   _projectRepository.Setup( x => x.Get( It.IsAny<Guid>() ) ).Returns( null as Project );

      //   var result = _controller.Edit( Guid.NewGuid() ) as HttpNotFoundResult;

      //   Assert.IsNotNull( result );
      //}

      //[TestMethod]
      //public void EditPost_CallRepositoryUpdateIfModelValid()
      //{
      //   var model = new ProjectEditorViewModel( Projects.ModelData[2] );

      //   _controller.Edit( model );

      //   _projectRepository.Verify( x => x.Update( It.Is<Project>( p => p.Id == model.Id ) ), Times.Once() );
      //}

      //[TestMethod]
      //public void EditPost_DoesNotCallRepositoryUpdateIfModelIsNotValid()
      //{
      //   var model = new ProjectEditorViewModel( Projects.ModelData[2] );

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   _controller.Edit( model );

      //   _projectRepository.Verify( x => x.Update( It.IsAny<Project>() ), Times.Never() );
      //}

      //[TestMethod]
      //public void EditPost_RedirectsToIndexIfModelIsValid()
      //{
      //   var model = new ProjectEditorViewModel( Projects.ModelData[2] );

      //   var result = _controller.Edit( model ) as RedirectToRouteResult;

      //   Assert.IsNotNull( result );
      //   Assert.AreEqual( 1, result.RouteValues.Count );

      //   object value;
      //   result.RouteValues.TryGetValue( "action", out value );
      //   Assert.AreEqual( "Index", value.ToString() );
      //}

      //[TestMethod]
      //public void EditPost_ReturnsViewIfModelIsNotValid()
      //{
      //   var model = new ProjectEditorViewModel( Projects.ModelData[2] );

      //   _controller.ModelState.AddModelError( "Test", "This is an error" );
      //   var result = _controller.Edit( model ) as ViewResult;

      //   Assert.IsNotNull( result );
      //   Assert.AreEqual( result.Model, model );
      //}

      //[TestMethod]
      //public void EditPost_PassesModelToValidator()
      //{
      //   var model = new ProjectEditorViewModel( Projects.ModelData[3] );

      //   _controller.Edit( model );

      //   _validator.Verify( x => x.ModelIsValid( model, TransactionType.Update ), Times.Once() );
      //}

      //[TestMethod]
      //public void EditPost_CopiesMessagesToModelStateIfValidatorReturnsFalse()
      //{
      //   var messages = CreateStockErrorMessages();
      //   var model = new ProjectEditorViewModel( Projects.ModelData[3] );

      //   _validator.SetupGet( x => x.Messages ).Returns( messages );
      //   _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( false );

      //   var result = _controller.Edit( model );

      //   Assert.AreEqual( messages.Count, _controller.ModelState.Count );
      //   foreach (var message in messages)
      //   {
      //      Assert.IsTrue( _controller.ModelState.ContainsKey( message.Key ) );
      //   }
      //   Assert.IsTrue( result is ViewResult );
      //}

      //[TestMethod]
      //public void EditPost_DoesNotCopyMessagesToModelStateIfValidatorReturnsTrue()
      //{
      //   var messages = CreateStockErrorMessages();
      //   var model = new ProjectEditorViewModel( Projects.ModelData[3] );

      //   _validator.SetupGet( x => x.Messages ).Returns( messages );
      //   _validator.Setup( x => x.ModelIsValid( model, It.IsAny<TransactionType>() ) ).Returns( true );

      //   var result = _controller.Edit( model );

      //   Assert.AreEqual( 0, _controller.ModelState.Count );
      //   Assert.IsNotNull( result );
      //   Assert.IsTrue( result is RedirectToRouteResult );
      //}
   }
}
