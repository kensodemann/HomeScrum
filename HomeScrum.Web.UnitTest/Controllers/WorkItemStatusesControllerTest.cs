﻿using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Web.Mvc;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Common.Utility;
using HomeScrum.Web.Controllers.Admin;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemStatusesControllerTest : SystemDataObjectControllerTestBase<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel>
   {
      protected override ICollection<WorkItemStatus> GetAllModels()
      {
         return WorkItemStatuses.ModelData;
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
         Database.Initialize();
         MapperConfig.RegisterMappings();
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         Database.Build();
         Users.Load();
         WorkItemStatuses.Load();
         base.InitializeTest();
      }

      public override ReadWriteController<WorkItemStatus, WorkItemStatusViewModel, WorkItemStatusEditorViewModel> CreateController()
      {
         var controller = new WorkItemStatusesController( new PropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>(), _logger.Object, NHibernateHelper.SessionFactory );
         controller.ControllerContext = new ControllerContext();

         return controller;
      }
   }
}
