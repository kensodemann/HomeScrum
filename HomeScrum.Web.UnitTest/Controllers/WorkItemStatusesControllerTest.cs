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
   public class WorkItemStatusesControllerTest : SystemDataObjectControllerTestBase<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>
   {
      protected override ICollection<WorkItemStatus> GetAllModels()
      {
         using (var session = Database.OpenSession())
         {
            return session.Query<WorkItemStatus>()
               .OrderBy( x => x.SortSequence )
               .ToList();
         }
      }

      protected override WorkItemStatus CreateNewModel()
      {
         return new WorkItemStatus()
         {
            Name = "New Work Item Status",
            Description = "New Work Item Status",
            IsPredefined = false,
            IsOpenStatus = true,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         ReadWriteControllerTestBase<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         CurrentSessionContext.Bind( Database.SessionFactory.OpenSession() );
         Database.Build();
         Users.Load();
         WorkItemStatuses.Load();
         base.InitializeTest();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         var session = CurrentSessionContext.Unbind( Database.SessionFactory );
         session.Dispose();
      }

      public override ReadWriteController<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel> CreateController()
      {
         var controller = new WorkItemStatusesController( new PropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }
   }
}
