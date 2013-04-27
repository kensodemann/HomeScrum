using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Extensions;
using HomeScrum.Common.TestData;

namespace HomeScrum.Web.UnitTest.Extensions
{
   [TestClass]
   public class CollectionExtensionTests
   {
      [TestInitialize]
      public void InitializeTest()
      {
         WorkItemTypes.CreateTestModelData( initializeIds: true );
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         Projects.CreateTestModelData( initializeIds: true );
         Users.CreateTestModelData( initializeIds: true );
      }

      [TestMethod]
      public void ToSelectList_ReturnsActiveItems()
      {
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList();

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), selectList.Count() );
         foreach (var item in selectList)
         {
            Assert.IsNotNull( WorkItemTypes.ModelData.FirstOrDefault( x => x.StatusCd == 'A' && x.Id.ToString() == item.Value ) );
         }
      }

      [TestMethod]
      public void ToSelectList_OrdersItemsByName()
      {
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList();

         string previousName = null;
         foreach (var item in selectList)
         {
            Assert.IsTrue( previousName == null ? true : String.Compare( previousName, item.Text, StringComparison.OrdinalIgnoreCase ) <= 0 );
            previousName = item.Text;
         }
      }

      [TestMethod]
      public void ToSelectedList_MarksItemSelected()
      {
         var selected = WorkItemTypes.ModelData.Where( x => x.StatusCd == 'A' ).ToArray()[2];
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList( selected.Id );

         foreach (var item in selectList)
         {
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void ToSelectedList_IncludesInactiveItemIfSelected()
      {
         var selected = WorkItemTypes.ModelData.First( x => x.StatusCd == 'I' );
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList( selected.Id );

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ) + 1, selectList.Count() );
         foreach (var item in selectList)
         {
            Assert.IsNotNull( WorkItemTypes.ModelData.FirstOrDefault( x => (x.StatusCd == 'A' || x.Id == selected.Id) && x.Id.ToString() == item.Value ) );
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }

      /* 
       * Project specific ToSelectList Tests
       * 
       * Many of these tests traverse the list starting with element 1.  This is because the
       * 0th element is always the "Not Assigned" element, and does not apply to the test.
       */
      [TestMethod]
      public void Project_ToSelectList_FirstElementTheNotAssignedElement()
      {
         var selectList = Projects.ModelData.ToArray().ToSelectList();

         var item = selectList.ElementAt( 0 );

         Assert.IsNull( item.Value );
         Assert.AreEqual( DisplayStrings.NotAssigned, item.Text );
         Assert.IsFalse( item.Selected );
      }

      [TestMethod]
      public void Project_ToSelectList_ReturnsActiveProjects()
      {
         var selectList = Projects.ModelData.ToArray().ToSelectList();

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.StatusCd == 'A' && x.Status.IsActive ) + 1, selectList.Count() );
         for (int i = 1; i < selectList.Count(); i++)
         {
            var item = selectList.ElementAt( i );
            Assert.IsNotNull( Projects.ModelData.FirstOrDefault( x => x.Status.IsActive && x.Id.ToString() == item.Value ) );
         }
      }

      [TestMethod]
      public void Project_ToSelectList_OrdersProjectssByName()
      {
         var selectList = Projects.ModelData.ToArray().ToSelectList();

         string previousName = null;
         for (int i = 1; i < selectList.Count(); i++)
         {
            var item = selectList.ElementAt( i );
            Assert.IsTrue( previousName == null ? true : String.Compare( previousName, item.Text, StringComparison.OrdinalIgnoreCase ) <= 0 );
            previousName = item.Text;
         }
      }

      [TestMethod]
      public void Project_ToSelectedList_MarksProjectSelected()
      {
         var selected = Projects.ModelData.Where( x => x.Status.StatusCd == 'A' && x.Status.IsActive ).ToArray()[1];
         var selectList = Projects.ModelData.ToArray().ToSelectList( selected.Id );

         foreach (var item in selectList)
         {
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void Project_ToSelectedList_IncludesInactiveProjectIfSelected()
      {
         var selected = Projects.ModelData.First( x => !x.Status.IsActive );
         var selectList = Projects.ModelData.ToArray().ToSelectList( selected.Id );

         Assert.AreEqual( Projects.ModelData.Count( x => x.Status.IsActive && x.Status.StatusCd == 'A' ) + 2, selectList.Count() );
         for (int i = 1; i < selectList.Count(); i++)
         {
            var item = selectList.ElementAt( i );
            Assert.IsNotNull( Projects.ModelData.FirstOrDefault( x => (x.Status.IsActive || x.Id == selected.Id) && x.Id.ToString() == item.Value ) );
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }
   }
}
