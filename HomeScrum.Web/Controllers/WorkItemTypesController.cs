using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Web.Controllers.Base;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Translators;
using Ninject;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HomeScrum.Web.Controllers
{
   public class WorkItemTypesController : ReadWriteController<WorkItemType, WorkItemTypeViewModel, WorkItemTypeEditorViewModel>
   {
      [Inject]
      public WorkItemTypesController( IRepository<WorkItemType> repository, IValidator<WorkItemType> validator, IPropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel> translator )
         : base( repository, validator, translator ) { }

      [HttpPost]
      public ActionResult UpdateSortOrders( IEnumerable<string> items )
      {
         // TODO: This code needs to be tested.  After that, it really needs to be refactored.
         //       Further, this would be really inefficient with large data sets.  OTOH, 
         //       this probably is not the type of thing that will be allowed with large
         //       data sets, so ignore that.
         int currentSortSequence = 0;
         foreach (var idToken in items)
         {
            var id = new Guid( idToken );
            var item = this.MainRepository.Get( id );
            currentSortSequence++;
            if (item != null && item.SortSequence != currentSortSequence)
            {
               item.SortSequence = currentSortSequence;
               MainRepository.Update( item );
            }
         }

         return new EmptyResult();
      }
   }
}
