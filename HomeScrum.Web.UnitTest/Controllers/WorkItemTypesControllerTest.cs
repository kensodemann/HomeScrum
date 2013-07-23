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
   public class WorkItemTypesControllerTest : SystemDataObjectControllerTestBase<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel>
   {
      protected override ICollection<WorkItemType> GetAllModels()
      {
         using (var session = Database.OpenSession())
         {
            return session.Query<WorkItemType>()
               .OrderBy( x => x.SortSequence )
               .ToList();
         }
      }

      protected override WorkItemType CreateNewModel()
      {
         return new WorkItemType()
         {
            Name = "New Work Item Type",
            Description = "New Work Item Type",
            IsPredefined = false,
            IsTask = false,
            StatusCd = 'A'
         };
      }

      [ClassInitialize]
      public static void InitiailizeTestClass( TestContext context )
      {
         ReadWriteControllerTestBase<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         CurrentSessionContext.Bind( Database.SessionFactory.OpenSession() );
         Database.Build();
         Users.Load();
         WorkItemTypes.Load();
         base.InitializeTest();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         var session = CurrentSessionContext.Unbind( Database.SessionFactory );
         session.Dispose();
      }

      public override ReadWriteController<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel> CreateController()
      {
         var controller = new WorkItemTypesController( new PropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel>(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
