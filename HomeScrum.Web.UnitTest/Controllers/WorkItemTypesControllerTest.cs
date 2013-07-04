﻿using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;


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
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         WorkItemTypes.CreateTestModelData( initializeIds: true );
         _controller = new WorkItemTypesController( _repository.Object, _validator.Object, new PropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel>(), _logger.Object );
         _controller.ControllerContext = new ControllerContext();
      }

   }
}
