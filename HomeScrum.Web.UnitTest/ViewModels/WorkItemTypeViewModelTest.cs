using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class WorkItemTypeViewModelTest
   {
      private WorkItemType CreateModel()
      {
         return new WorkItemType()
         {
            Id = Guid.NewGuid(),
            Name = "The Name",
            Description = "The Description",
            IsPredefined = 'Y',
            IsTask = 'N',
            StatusCd = 'A'
         };
      }


      [TestMethod]
      public void DefaultConstructor_CreatesEmptyModel()
      {
         var viewModel = new WorkItemTypeViewModel();

         Assert.IsNotNull( viewModel.Model );
         Assert.AreEqual( default( Guid ), viewModel.Model.Id );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel.Model.Name ) );
         Assert.IsTrue( string.IsNullOrEmpty( viewModel.Model.Description ) );
      }

      [TestMethod]
      public void ContructorSetsTheModel()
      {
         var model = CreateModel();
         var viewModel = new WorkItemTypeViewModel( model );

         Assert.AreEqual( model, viewModel.Model );
      }

      [TestMethod]
      public void IsTaskTrue_IfModelIsTaskY()
      {
         var model = CreateModel();
         model.IsTask = 'Y';

         var viewModel = new WorkItemTypeViewModel( model );

         Assert.IsTrue( viewModel.IsTask );
      }

      [TestMethod]
      public void IsTaskFalse_IfModelIsTaskN()
      {
         var model = CreateModel();
         model.IsTask = 'N';

         var viewModel = new WorkItemTypeViewModel( model );

         Assert.IsFalse( viewModel.IsTask );
      }

      [TestMethod]
      public void SettingIsTaskTrue_SetsModelIsTaskY()
      {
         var viewModel = new WorkItemTypeViewModel();

         viewModel.IsTask = true;

         Assert.AreEqual( 'Y', viewModel.Model.IsTask );
      }

      [TestMethod]
      public void SettingIsTaskFalse_SetsModelIsTaskN()
      {
         var viewModel = new WorkItemTypeViewModel();

         viewModel.IsTask = false;

         Assert.AreEqual( 'N', viewModel.Model.IsTask );
      }
   }
}
