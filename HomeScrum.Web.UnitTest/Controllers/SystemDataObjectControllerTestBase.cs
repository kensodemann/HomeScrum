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
      private SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT> MyController
      {
         get
         {
            return _controller as SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT>;
         }
      }


      [TestMethod]
      public void UpdateSortOrders_ReturnsEmptyResult()
      {
         var ids = TestObjectIdList();
         SetupRepositoryGets();

         var result = MyController.UpdateSortOrders( ids ) as EmptyResult;

         Assert.IsNotNull( result );
      }

      [TestMethod]
      public void UpdateSortOrders_DoesNotUpdateSortOrders_IfNoOrdersHaveChanged()
      {
         var ids = TestObjectIdList();
         SetupRepositoryGets();

         var result = MyController.UpdateSortOrders( ids );

         _repository.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Never() );
      }

      [TestMethod]
      public void UpdateSortOrders_UpdatesSortOrders_IfNodeOrdersChanged()
      {
         var ids = TestObjectIdList();
         var swapId = ids[2];
         ids[2] = ids[4];
         ids[4] = swapId;

         SetupRepositoryGets();

         var results = MyController.UpdateSortOrders( ids );

         _repository.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Exactly( 2 ) );
         _repository.Verify( x => x.Update( It.Is<ModelT>( m => m.Id.ToString() == ids[2] && m.SortSequence == 3 ) ), Times.Once() );
         _repository.Verify( x => x.Update( It.Is<ModelT>( m => m.Id.ToString() == ids[4] && m.SortSequence == 5 ) ), Times.Once() );
      }

      [TestMethod]
      public void UpdateSortOrders_DoesNotUpdateIdsNotInRepository()
      {
         var ids = TestObjectIdList();
         SetupRepositoryGets();
         var newId = Guid.NewGuid();
         _repository.Setup( x => x.Get( newId ) ).Returns( null as ModelT );
         ids.Add( newId.ToString() );

         var results = MyController.UpdateSortOrders( ids );

         _repository.Verify( x => x.Update( It.IsAny<ModelT>() ), Times.Never() );
      }

      #region Private Helpers
      private List<string> TestObjectIdList()
      {
         return GetAllModels()
            .OrderBy( x => x.SortSequence )
            .Select( x => x.Id.ToString() )
            .ToList();
      }

      private void SetupRepositoryGets()
      {
         foreach (var item in GetAllModels())
         {
            _repository
               .Setup( x => x.Get( item.Id ) )
               .Returns( item );
         }
      }
      #endregion
   }
}
