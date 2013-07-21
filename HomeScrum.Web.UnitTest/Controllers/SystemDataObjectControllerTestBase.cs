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
      where EditorViewModelT : SystemDomainObjectViewModel, new()
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

         var ids = TestObjectIdList();
         //SetupSessionGets();

         var result = controller.UpdateSortOrders( ids ) as EmptyResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void UpdateSortOrders_DoesNotUpdateSortOrders_IfNoOrdersHaveChanged()
      {
         var controller = CreateController() as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;
         var ids = TestObjectIdList();
         //SetupSessionGets();

         var result = controller.UpdateSortOrders( ids );

         //_session.Verify( x => x.BeginTransaction(), Times.Once() );
         //_transaction.Verify( x => x.Commit(), Times.Once() );
         //_session.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void UpdateSortOrders_UpdatesSortOrders_IfNodeOrdersChanged()
      {
         var controller = CreateController() as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;
         var ids = TestObjectIdList();
         var swapId = ids[2];
         ids[2] = ids[4];
         ids[4] = swapId;

         //SetupSessionGets();

         var results = controller.UpdateSortOrders( ids );

         //_session.Verify( x => x.BeginTransaction(), Times.Once() );
         //_transaction.Verify( x => x.Commit(), Times.Once() );
         //_session.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Exactly( 2 ) );
         //_session.Verify( x => x.Update( It.Is<ModelT>( m => m.Id.ToString() == ids[2] && m.SortSequence == 3 ) ), Times.Once() );
         //_session.Verify( x => x.Update( It.Is<ModelT>( m => m.Id.ToString() == ids[4] && m.SortSequence == 5 ) ), Times.Once() );
      }

      [TestMethod]
      public void UpdateSortOrders_DoesNotUpdateIdsNot()
      {
         var controller = CreateController() as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;
         var ids = TestObjectIdList();
         //SetupSessionGets();
         var newId = Guid.NewGuid();
         //_session.Setup( x => x.Get<ModelT>( newId ) ).Returns( null as ModelT );
         ids.Add( newId.ToString() );

         var results = controller.UpdateSortOrders( ids );

         //_session.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Never() );
      }

      #region Private Helpers
      private List<string> TestObjectIdList()
      {
         return GetAllModels()
            .OrderBy( x => x.SortSequence )
            .Select( x => x.Id.ToString() )
            .ToList();
      }
      #endregion
   }
}
