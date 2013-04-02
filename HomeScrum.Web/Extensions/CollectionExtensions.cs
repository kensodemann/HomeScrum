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
      public class CaseInsensitiveComparer : IComparer<string>
      {
         public int Compare( string x, string y )
         {
            return string.Compare( x, y, StringComparison.OrdinalIgnoreCase );
         }
      }

      public static IEnumerable<SelectListItem> ToSelectList<ModelT>( this IEnumerable<ModelT> collection, Guid selectedId )
         where ModelT : SystemDataObject
      {
         return collection
            .OrderBy( x => x.Name, new CaseInsensitiveComparer() )
            .Where( x => x.StatusCd == 'A' )
            .Select( item => new SelectListItem()
                             {
                                Value = item.Id.ToString(),
                                Text = item.Name,
                                Selected = item.Id == selectedId
                             } );
      }
   }
}