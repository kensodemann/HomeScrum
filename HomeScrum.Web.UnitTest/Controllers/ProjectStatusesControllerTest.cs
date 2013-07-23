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
         using (var session = Database.OpenSession())
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
         ReadWriteControllerTestBase<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         CurrentSessionContext.Bind( Database.SessionFactory.OpenSession() );
         Database.Build();
         Users.Load();
         ProjectStatuses.Load();
         base.InitializeTest();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         var session = CurrentSessionContext.Unbind( Database.SessionFactory );
         session.Dispose();
      }

      public override ReadWriteController<ProjectStatus, ProjectStatusViewModel, ProjectStatusEditorViewModel> CreateController()
      {
         var controller = new ProjectStatusesController( new PropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
