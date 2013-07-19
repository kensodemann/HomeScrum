using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using HomeScrum.Common.Utility;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Controllers.Admin;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemTypesControllerTest : SystemDataObjectControllerTestBase<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel>
   {
      protected override ICollection<WorkItemType> GetAllModels()
      {
         return WorkItemTypes.ModelData;
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
         Database.Initialize();
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         Database.Build();
         Users.Load();
         WorkItemTypes.Load();
         base.InitializeTest();
      }


      public override ReadWriteController<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel> CreateController()
      {
         var controller = new WorkItemTypesController( new PropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel>(), _logger.Object, NHibernateHelper.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
