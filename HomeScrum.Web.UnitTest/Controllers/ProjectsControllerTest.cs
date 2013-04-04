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
      private Mock<IRepository<Project, Guid>> _projectRepository;
      private Mock<IRepository<ProjectStatus, Guid>> _projectStatusRepository;
      private Mock<IValidator<Project>> _validator;
      private ProjectsController _controller;

      [TestInitialize]
      public virtual void InitializeTest()
      {
         Users.CreateTestModelData( initializeIds: true );
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         Projects.CreateTestModelData( initializeIds: true );

         _projectRepository = new Mock<IRepository<Project, Guid>>();
         _projectStatusRepository = new Mock<IRepository<ProjectStatus, Guid>>();
         _projectStatusRepository.Setup( x => x.GetAll() ).Returns( ProjectStatuses.ModelData );
         _projectRepository.Setup( x => x.GetAll() ).Returns( Projects.ModelData );

         _validator = new Mock<IValidator<Project>>();
         _validator.Setup( x => x.ModelIsValid( It.IsAny<Project>(), It.IsAny<TransactionType>() ) ).Returns( true );

         _controller = new ProjectsController( _projectRepository.Object, _projectStatusRepository.Object, _validator.Object );
      }

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
         Assert.AreEqual( model, view.Model );
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

         Assert.AreEqual( ProjectStatuses.ModelData.Count( x => x.AllowUse ), model.ProjectStatuses.Count() );
         foreach (var item in model.ProjectStatuses)
         {
            var status = ProjectStatuses.ModelData.First( x => x.Id.ToString() == item.Value );
            Assert.AreEqual( status.Name, item.Text );
            Assert.IsFalse( item.Selected );
         }
      }
   }
}
