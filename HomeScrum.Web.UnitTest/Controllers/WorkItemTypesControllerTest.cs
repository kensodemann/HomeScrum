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
         var models = _session.Query<WorkItemType>()
            .OrderBy( x => x.SortSequence )
            .ToList();

         _session.Clear();
         return models;
      }

      protected override WorkItemType CreateNewModel()
      {
         return new WorkItemType()
         {
            Name = "New Work Item Type",
            Description = "New Work Item Type",
            IsPredefined = false,
            Category = WorkItemTypeCategory.BacklogItem,
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
         base.InitializeTest();

         Database.Build( _session );
         Users.Load( _sessionFactory.Object );
         WorkItemTypes.Load( _sessionFactory.Object );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }

      public override ReadWriteController<WorkItemType, WorkItemTypeEditorViewModel> CreateController()
      {
         var controller = new WorkItemTypesController( new PropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel>(), _logger.Object, _sessionFactory.Object );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }
   }
}
