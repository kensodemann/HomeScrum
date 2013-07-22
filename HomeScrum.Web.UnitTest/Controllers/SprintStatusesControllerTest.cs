using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
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
   public class SprintStatusesControllerTest : SystemDataObjectControllerTestBase<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel>
   {
      protected override ICollection<SprintStatus> GetAllModels()
      {
         using (var session = Database.OpenSession())
         {
            return session.Query<SprintStatus>()
               .OrderBy( x => x.SortSequence )
               .ToList();
         }
      }

      protected override SprintStatus CreateNewModel()
      {
         return new SprintStatus()
         {
            Name = "New Sprint Status",
            Description = "New Sprint Status",
            IsPredefined = false,
            IsOpenStatus = true,
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
         SprintStatuses.Load();
         base.InitializeTest();
      }

      public override Web.Controllers.Base.ReadWriteController<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel> CreateController()
      {
         var controller = new SprintStatusesController( new PropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel>(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
