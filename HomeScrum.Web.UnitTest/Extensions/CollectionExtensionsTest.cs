using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Web.Extensions;
using HomeScrum.Common.TestData;

namespace HomeScrum.Web.UnitTest.Extensions
{
   [TestClass]
   public class CollectionExtensionsTest
   {
      [TestInitialize]
      public void InitializeTest()
      {
         Users.CreateTestModelData( initializeIds: true );
         WorkItemTypes.CreateTestModelData( initializeIds: true );
         WorkItemStatuses.CreateTestModelData( initializeIds: true );
         ProjectStatuses.CreateTestModelData( initializeIds: true );
         Projects.CreateTestModelData( initializeIds: true );
      }

      [TestMethod]
      public void ToSelectList_ReturnsActiveItems()
      {
         var selectList = WorkItemStatuses.ModelData.ToArray().ToSelectList();

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ), selectList.Count() );
         foreach (var item in selectList)
         {
            Assert.IsNotNull( WorkItemStatuses.ModelData.FirstOrDefault( x => x.StatusCd == 'A' && x.Id.ToString() == item.Value ) );
         }
      }

      [TestMethod]
      public void ToSelectedList_MarksItemSelected()
      {
         var selected = WorkItemStatuses.ModelData.Where( x => x.StatusCd == 'A' ).ToArray()[2];
         var selectList = WorkItemStatuses.ModelData.ToArray().ToSelectList( selected.Id );

         foreach (var item in selectList)
         {
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void ToSelectedList_IncludesInactiveItemIfSelected()
      {
         var selected = WorkItemStatuses.ModelData.First( x => x.StatusCd == 'I' );
         var selectList = WorkItemStatuses.ModelData.ToArray().ToSelectList( selected.Id );

         Assert.AreEqual( WorkItemStatuses.ModelData.Count( x => x.StatusCd == 'A' ) + 1, selectList.Count() );
         foreach (var item in selectList)
         {
            Assert.IsNotNull( WorkItemStatuses.ModelData.FirstOrDefault( x => (x.StatusCd == 'A' || x.Id == selected.Id) && x.Id.ToString() == item.Value ) );
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

         Assert.AreEqual( default( Guid ).ToString(), item.Value );
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
      public void Project_ToSelectList_OrdersProjectsByName()
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

      /* 
       * User specific ToSelectList Tests
       */
      [TestMethod]
      public void User_ToSelectList_FirstElementTheNotAssignedElement_IfAllowed()
      {
         var selectList = Users.ModelData.ToArray().ToSelectList( true );

         var item = selectList.ElementAt( 0 );

         Assert.AreEqual( default( Guid ).ToString(), item.Value );
         Assert.AreEqual( DisplayStrings.NotAssigned, item.Text );
         Assert.IsFalse( item.Selected );
      }

      [TestMethod]
      public void User_ToSelectList_FirstElementNotTheNotAssignedElement_IfNotAllowed()
      {
         var selectList = Users.ModelData.ToArray().ToSelectList( false );

         var item = selectList.ElementAt( 0 );

         Assert.IsNotNull( item.Value );
         Assert.AreNotEqual( DisplayStrings.NotAssigned, item.Text );
      }

      [TestMethod]
      public void User_ToSelectList_ReturnsActiveUsers()
      {
         var selectList = Users.ModelData.ToArray().ToSelectList( false );

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ), selectList.Count() );
         foreach (var item in selectList)
         {
            Assert.IsNotNull( Users.ModelData.FirstOrDefault( x => x.StatusCd == 'A' && x.Id.ToString() == item.Value ) );
         }
      }

      [TestMethod]
      public void User_ToSelectList_OrdersUsersByName()
      {
         var selectList = Users.ModelData.ToArray().ToSelectList( false );

         string previousName = null;
         foreach (var item in selectList)
         {
            Assert.IsTrue( previousName == null ? true : String.Compare( previousName, item.Text, StringComparison.OrdinalIgnoreCase ) <= 0 );
            previousName = item.Text;
         }
      }

      [TestMethod]
      public void User_ToSelectedList_MarksUserSelected()
      {
         var selected = Users.ModelData.Where( x => x.StatusCd == 'A' ).ToArray()[1];
         var selectList = Users.ModelData.ToArray().ToSelectList( false, selected.Id );

         foreach (var item in selectList)
         {
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void User_ToSelectedList_IncludesInactiveUserIfSelected()
      {
         var selected = Users.ModelData.First( x => x.StatusCd == 'I' );
         var selectList = Users.ModelData.ToArray().ToSelectList(false, selected.Id );

         Assert.AreEqual( Users.ModelData.Count( x => x.StatusCd == 'A' ) + 1, selectList.Count() );
         foreach(var item in selectList)
         {
            Assert.IsNotNull( Users.ModelData.FirstOrDefault( x => (x.StatusCd == 'A' || x.Id == selected.Id) && x.Id.ToString() == item.Value ) );
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }

      /* 
       * Work Item Type specific ToSelectList Tests
       */
      [TestMethod]
      public void WorkItemType_ToSelectList_ReturnsActiveWorkItemTypes()
      {
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList();

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), selectList.Count() );
         foreach (var item in selectList)
         {
            Assert.IsNotNull( WorkItemTypes.ModelData.FirstOrDefault( x => x.StatusCd == 'A' && x.Id.ToString() == item.Value ) );
         }
      }

      [TestMethod]
      public void WorkItemType_ToSelectedList_MarksItemSelected()
      {
         var selected = WorkItemTypes.ModelData.Where( x => x.StatusCd == 'A' ).ToArray()[2];
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList( selected.Id );

         foreach (var item in selectList)
         {
            Assert.IsTrue( item.Value == selected.Id.ToString() ? item.Selected : !item.Selected );
         }
      }

      [TestMethod]
      public void WorkItemType_ToSelectedList_IncludesInactiveItemIfSelected()
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
   }
}
