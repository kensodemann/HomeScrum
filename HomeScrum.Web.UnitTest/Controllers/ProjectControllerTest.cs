using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using System.Collections.Generic;
using HomeScrum.Common.TestData;
using HomeScrum.Web.Controllers;
using System.Web.Mvc;

namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class ProjectControllerTest : DomainObjectControllerTestBase<Project>
   {
      protected override ICollection<Project> GetAllModels()
      {
         return Projects.ModelData;
      }

      protected override Project CreateNewModel()
      {
         return new Project()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            LastModifiedUserRid = Users.ModelData[0].Id,
            ProjectStatus = ProjectStatuses.ModelData[0]
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         Users.CreateTestModelData();
         ProjectStatuses.CreateTestModelData();
         Projects.CreateTestModelData();
         _controller = new ProjectsController( _repository.Object, _validator.Object );
      }

      [TestMethod]
      public new void EditGet_ReturnsViewWithModel()
      {
         var model = GetAllModels().ToArray()[3];
         _repository.Setup( x => x.Get( model.Id ) ).Returns( model );

         var result = _controller.Edit( model.Id ) as ViewResult;

         Assert.IsNotNull( result );
         Assert.IsNotNull( result.Model );
         Assert.AreEqual( model.Id,  ((Project)result.Model).Id );
         Assert.AreEqual( model.Name, ((Project)result.Model).Name );
         Assert.AreEqual( model.Description, ((Project)result.Model).Description );
      }
   }
}
