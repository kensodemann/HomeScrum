using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Controllers.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;

namespace HomeScrum.Web.UnitTest.Controllers
{
   public abstract class SystemDataObjectControllerTestBase<ModelT, ViewModelT, EditorViewModelT> : ReadWriteControllerTestBase<ModelT, ViewModelT, EditorViewModelT>
      where ModelT : SystemDomainObject, new()
      where ViewModelT : SystemDomainObjectViewModel, new()
      where EditorViewModelT : SystemDomainObjectViewModel, IEditorViewModel, new()
   {
      [TestMethod]
      public void Index_ReturnsItemsInSortSeqOrder()
      {
         var controller = CreateController();

         var view = controller.Index() as ViewResult;
         var model = view.Model as IEnumerable<SystemDomainObjectViewModel>;

         var expectedItems = GetAllModels().OrderBy( x => x.SortSequence );
         var itemCount = model.Count();

         for (int i = 0; i < itemCount; i++)
         {
            Assert.AreEqual( expectedItems.ElementAt( i ).Id, model.ElementAt( i ).Id );
         }
      }

      [TestMethod]
      public void UpdateSortOrders_ReturnsEmptyResult()
      {
         var controller = CreateController() as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;

         var ids = GetObjectIdList();

         var result = controller.UpdateSortOrders( ids ) as EmptyResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void UpdateSortOrders_DoesNotUpdateSortOrders_IfNoOrdersHaveChanged()
      {
         var controller = CreateController() as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;
         var expectedIds = GetObjectIdList();

         var result = controller.UpdateSortOrders( expectedIds );

         var actualIds = GetObjectIdList();

         for (int i = 0; i < actualIds.Count; i++)
         {
            Assert.AreEqual( expectedIds[i], actualIds[i], String.Format( "Index: %d", i ) );
         }
      }

      [TestMethod]
      public void UpdateSortOrders_UpdatesSortOrders_IfNodeOrdersChanged()
      {
         var controller = CreateController() as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;
         var expectedIds = GetObjectIdList();
         var swapId = expectedIds[2];
         expectedIds[2] = expectedIds[4];
         expectedIds[4] = swapId;

         var results = controller.UpdateSortOrders( expectedIds );

         var actualIds = GetObjectIdList();

         for (int i = 0; i < actualIds.Count; i++)
         {
            Assert.AreEqual( expectedIds[i], actualIds[i], String.Format( "Index: {0}", i ) );
         }
      }

      [TestMethod]
      public void UpdateSortOrders_DoesNotUpdateIdsNot()
      {
         var controller = CreateController() as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;
         var expectedIds = GetObjectIdList();
         var testIds = GetObjectIdList();
         var newId = Guid.NewGuid();

         testIds.Add( newId.ToString() );

         var results = controller.UpdateSortOrders( testIds );

         var actualIds = GetObjectIdList();

         for (int i = 0; i < actualIds.Count; i++)
         {
            Assert.AreEqual( expectedIds[i], actualIds[i], String.Format( "Index: {0}", i ) );
         }
      }

      #region Private Helpers
      private List<string> GetObjectIdList()
      {
         return GetAllModels()
            .OrderBy( x => x.SortSequence )
            .Select( x => x.Id.ToString() )
            .ToList();
      }
      #endregion
   }
}
