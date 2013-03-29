using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Controllers;
using HomeScrum.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace HomeScrum.Web.UnitTest.Controllers
{
   [TestClass]
   public class WorkItemTypeesControllerTest : DomainObjectControllerTestBase<WorkItemType, WorkItemTypeEditorViewModel>
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
            AllowUse = true
         };
      }

      [TestInitialize]
      public override void InitializeTest()
      {
         base.InitializeTest();
         WorkItemTypes.CreateTestModelData();
         _controller = new WorkItemTypesController( _repository.Object, _validator.Object );
      }

   }
}
