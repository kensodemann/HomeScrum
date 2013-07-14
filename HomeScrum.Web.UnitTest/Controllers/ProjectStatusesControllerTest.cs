using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Common.Utility;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class ProjectStatusesControllerTest : SystemDataObjectControllerTestBase<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>
   {
      protected override ICollection<ProjectStatus> GetAllModels()
      {
         return ProjectStatuses.ModelData;
      }

      protected override ProjectStatus CreateNewModel()
      {
         return new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            IsPredefined = false,
            IsActive = true,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         Database.Initialize();
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         Database.Build();
         ProjectStatuses.Load();
         base.InitializeTest();
      }

      public override ReadWriteController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel> CreateDatabaseConnectedController()
      {
         var controller = new ProjectStatusesController( _validator.Object, new PropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>(), _logger.Object, NHibernateHelper.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }

      public override ReadWriteController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel> CreateDatabaseMockedController()
      {
         var controller = new ProjectStatusesController( _validator.Object, new PropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
