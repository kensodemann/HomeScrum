using HomeScrum.Common.TestData;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
         using (var session = NHibernateHelper.OpenSession())
         {
            return session.Query<ProjectStatus>()
               .OrderBy( x => x.SortSequence )
               .ToList();
         }
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
         Users.Load();
         ProjectStatuses.Load();
         base.InitializeTest();
      }

      public override ReadWriteController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel> CreateController()
      {
         var controller = new ProjectStatusesController( new PropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>(), _logger.Object, NHibernateHelper.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
