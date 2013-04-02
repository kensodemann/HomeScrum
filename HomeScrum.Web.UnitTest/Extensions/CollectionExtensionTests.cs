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
         WorkItemTypes.CreateTestModelData();
      }


      [TestMethod]
      public void ToSelectList_ReturnsActiveItems()
      {
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList( default( Guid ) );

         Assert.AreEqual( WorkItemTypes.ModelData.Count( x => x.StatusCd == 'A' ), selectList.Count() );
         foreach (var item in selectList)
         {
            Assert.IsNotNull( WorkItemTypes.ModelData.FirstOrDefault( x => x.Id.ToString() == item.Value ) );
         }
      }

      [TestMethod]
      public void ToSelectList_OrdersItemsByName()
      {
         var selectList = WorkItemTypes.ModelData.ToArray().ToSelectList( default( Guid ) );

         string previousName = null;
         foreach (var item in selectList)
         {
            Assert.IsTrue( previousName == null ? true : String.Compare( previousName, item.Text, StringComparison.OrdinalIgnoreCase ) <= 0 );
            previousName = item.Text;
         }
      }
   }
}
