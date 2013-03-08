using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Models;

namespace HomeScrum.Web.UnitTest.ViewModels
{
   [TestClass]
   public class DataObjectBaseViewModelTest
   {
      [TestMethod]
      public void DefaultConstructor_CreatesEmptyModel()
      {
         var viewModel = new DataObjectBaseViewModel<DataObjectBase>();

         Assert.IsNotNull( viewModel.Model );
         Assert.AreEqual( default( Guid ), viewModel.Model.Id );
         Assert.IsTrue( String.IsNullOrEmpty( viewModel.Model.Name ) );
         Assert.IsTrue( string.IsNullOrEmpty( viewModel.Model.Description ) );
      }
   }
}
