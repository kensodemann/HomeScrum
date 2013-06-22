﻿using HomeScrum.Data.Domain;
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
         // The repository defaults to the user defined sort order, so just use that.
         return collection
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

      public static IEnumerable<SelectListItem> ToSelectList( this IEnumerable<User> collection, bool allowUnassigned, Guid selectedId = default(Guid) )
      {
         var selectList = new List<SelectListItem>();

         if (allowUnassigned)
         {
            AddEmptyItem( selectList );
         }

         selectList.AddRange(
            collection
               .OrderBy( x => x.LastName + x.FirstName, new CaseInsensitiveComparer() )
               .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
               .Select( item => new SelectListItem()
                                {
                                   Value = item.Id.ToString(),
                                   Text = (String.IsNullOrWhiteSpace( item.LastName ) ? "" : item.LastName + ", ") + item.FirstName,
                                   Selected = item.Id == selectedId
                                } ) );

         return selectList;
      }

      public static IEnumerable<SelectListItemWithAttributes> ToSelectList( this IEnumerable<WorkItemType> collection, Guid selectedId = default( Guid ) )
      {
         // The repository defaults to the user defined sort order, so just use that.
         return collection
            .Where( x => x.StatusCd == 'A' || x.Id == selectedId )
            .Select( item => new SelectListItemWithAttributes()
            {
               Value = item.Id.ToString(),
               Text = item.Name,
               Selected = item.Id == selectedId,
               DataAttributes = new Dictionary<string, string>()
               {
                  { "IsAssignable", item.IsTask ? "True" : "False" }
               }
            } );
      }

      private static void AddEmptyItem( List<SelectListItem> selectList )
      {
         selectList.Add( new SelectListItem()
         {
            Value = default( Guid ).ToString(),
            Text = DisplayStrings.NotAssigned,
            Selected = false
         } );
      }
   }
}