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

      public static IEnumerable<SelectListItem> ToSelectList<ModelT>( this IEnumerable<ModelT> collection, Guid selectedId = default( Guid ) )
         where ModelT : SystemDomainObject
      {
         return collection
            .OrderBy( x => x.Name, new CaseInsensitiveComparer() )
            .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
            .Select( item => new SelectListItem()
                             {
                                Value = item.Id.ToString(),
                                Text = item.Name,
                                Selected = item.Id == selectedId
                             } );
      }

      public static IEnumerable<SelectListItem> ToSelectList( this IEnumerable<Project> collection, Guid selectedId = default(Guid) )
      {
         var selectList = new List<SelectListItem>();

         AddEmptyItem( selectList );

         selectList.AddRange(
            collection
               .OrderBy( x => x.Name, new CaseInsensitiveComparer() )
               .Where( x => (x.Status.StatusCd == 'A' && x.Status.IsActive) || x.Id == selectedId )
               .Select( item => new SelectListItem()
                                {
                                   Value = item.Id.ToString(),
                                   Text = item.Name,
                                   Selected = item.Id == selectedId
                                } ) );



         return selectList;
      }

      private static void AddEmptyItem( List<SelectListItem> selectList )
      {
         selectList.Add( new SelectListItem()
         {
            Value = null,
            Text = DisplayStrings.NotAssigned,
            Selected = false
         } );
      }
   }
}