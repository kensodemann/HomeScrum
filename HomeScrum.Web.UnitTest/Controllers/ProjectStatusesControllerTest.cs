using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Context;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class ProjectStatusesControllerTest : SystemDataObjectControllerTestBase<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>
   {
      protected override ICollection<ProjectStatus> GetAllModels()
      {
         var models = _session.Query<ProjectStatus>()
            .OrderBy( x => x.SortSequence )
            .ToList();

         _session.Clear();
         return models;
      }

      protected override ProjectStatus CreateNewModel()
      {
         return new ProjectStatus()
         {
            Name = "New Project Status",
            Description = "New Project Status",
            IsPredefined = false,
            Category = ProjectStatusCategory.Active,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         ReadWriteControllerTestBase<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         
         Database.Build( _session );
         Users.Load( _sessionFactory.Object );
         ProjectStatuses.Load( _sessionFactory.Object );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }

      public override ReadWriteController<ProjectStatus, ProjectStatusEditorViewModel> CreateController()
      {
         var controller = new ProjectStatusesController( new PropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }
   }
}
