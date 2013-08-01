﻿using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers.Admin;
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
         ReadWriteControllerTestBase<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel>.InitializeClass( context );
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         CurrentSessionContext.Bind( Database.SessionFactory.OpenSession() );
         Database.Build();
         Users.Load();
         SprintStatuses.Load();
         base.InitializeTest();
      }

      [TestCleanup]
      public void CleanupTest()
      {
         var session = CurrentSessionContext.Unbind( Database.SessionFactory );
         session.Dispose();
      }

      public override Web.Controllers.Base.ReadWriteController<SprintStatus, SprintStatusViewModel, SprintStatusEditorViewModel> CreateController()
      {
         var controller = new SprintStatusesController( new PropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel>(), _logger.Object, Database.SessionFactory );
         controller.ControllerContext = _controllerConext.Object;

         return controller;
      }
   }
}
