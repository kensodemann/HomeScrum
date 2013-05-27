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
         var result = MyController.UpdateSortOrders( null ) as EmptyResult;

         Assert.IsNotNull( result );
      }
   }
}
