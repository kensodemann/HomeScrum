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
   }
}
