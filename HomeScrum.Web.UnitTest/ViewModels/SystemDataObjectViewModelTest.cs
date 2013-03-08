using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class SystemDataObjectViewModelTest
   {
      private SystemDataObject CreateModel()
      {
         return new SystemDataObject()
         {
            Id = Guid.NewGuid(),
            Name = "The Name",
            Description = "The Description",
            IsPredefined = 'Y',
            StatusCd = 'A'
         };
      }


      [TestMethod]
      public void IsActiveTrue_IfModelStatusIsA()
      {
         var model = CreateModel();
         model.StatusCd = 'A';

         var viewModel = new SystemDataObjectViewModel<SystemDataObject>( model );

         Assert.IsTrue( viewModel.IsActive );
      }

      [TestMethod]
      public void IsActiveFalse_IfModleStatusIsNotA()
      {
         var model = CreateModel();
         model.StatusCd = 'I';

         var viewModel = new SystemDataObjectViewModel<SystemDataObject>( model );

         Assert.IsFalse( viewModel.IsActive );
      }

      [TestMethod]
      public void SettingIsActiveFalse_SetsStatusCdToI()
      {
         var viewModel = new SystemDataObjectViewModel<SystemDataObject>();

         viewModel.IsActive = false;

         Assert.AreEqual( 'I', viewModel.Model.StatusCd );
      }

      [TestMethod]
      public void SettingIsActiveTrue_SetsStatusCdToA()
      {
         var viewModel = new SystemDataObjectViewModel<SystemDataObject>();

         viewModel.IsActive = true;

         Assert.AreEqual( 'A', viewModel.Model.StatusCd );
      }
   }
}
