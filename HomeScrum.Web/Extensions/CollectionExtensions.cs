using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Extensions
{
   public static class CollectionExtensions
   {
      public static IEnumerable<SelectListItem> ToSelectList<ModelT>( this IEnumerable<ModelT> collection, Guid selectedId )
         where ModelT : SystemDataObject
      {
         return collection
            .Where( x => x.StatusCd == 'A' )
            .Select( item => new SelectListItem()
                             {
                                Value = item.Id.ToString(),
                                Text = item.Name
                             } );
      }
   }
}