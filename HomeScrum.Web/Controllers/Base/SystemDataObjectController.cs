using AutoMapper;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers.Base
{
   public class SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT> : ReadWriteController<ModelT, ViewModelT, EditorViewModelT>
      where ModelT : SystemDomainObject
      where ViewModelT : SystemDomainObjectViewModel
      where EditorViewModelT : SystemDomainObjectViewModel, new()
   {
      public SystemDataObjectController( IRepository<ModelT> mainRepository, IValidator<ModelT> validator, IPropertyNameTranslator<ModelT, EditorViewModelT> translator, ILogger logger )
         : base( mainRepository, validator, translator, logger ) { }


      //
      // POST: /ModelTs/UpdateSortOrders
      [HttpPost]
      public ActionResult UpdateSortOrders( IEnumerable<string> itemIds )
      {
         var idIndex = 0;
         foreach (var id in itemIds)
         {
            var item = MainRepository.Get( new Guid( id ) );
            idIndex++;
            if (item != null && item.SortSequence != idIndex)
            {
               item.SortSequence = idIndex;
               MainRepository.Update( item );
            }
         }

         return new EmptyResult();
      }
   }
}