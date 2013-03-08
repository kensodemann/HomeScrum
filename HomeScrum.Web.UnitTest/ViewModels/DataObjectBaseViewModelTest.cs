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

      [TestMethod]
      public void ContructorSetsTheModel()
      {
         var model = new DataObjectBase()
         {
            Id = Guid.NewGuid(),
            Name = "The Name",
            Description = "The Description"
         };
         var viewModel = new DataObjectBaseViewModel<DataObjectBase>( model );

         Assert.AreEqual( model, viewModel.Model );
      }

      [TestMethod]
      public void Name_ReturnsModelName()
      {
         var model = new DataObjectBase()
         {
            Id = Guid.NewGuid(),
            Name = "The Name",
            Description = "The Description"
         };
         var viewModel = new DataObjectBaseViewModel<DataObjectBase>( model );

         Assert.AreEqual( model.Name, viewModel.Name );
      }

      [TestMethod]
      public void Name_SetsModelName()
      {
         var model = new DataObjectBase()
         {
            Id = Guid.NewGuid(),
            Name = "The Name",
            Description = "The Description"
         };
         var viewModel = new DataObjectBaseViewModel<DataObjectBase>( model );
         viewModel.Name = "Something Else";

         Assert.AreEqual( "Something Else", viewModel.Model.Name );
      }
   }
}
