﻿using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.Extensions
{
   public static class QueryProjections
   {
      public static IList<DomainObjectViewModel> SelectDomainObjectViewModels<SourceT>( this IQueryable<SourceT> query )
         where SourceT : DomainObjectBase
      {
         return query.Select( x => new DomainObjectViewModel()
                                   {
                                      Id = x.Id,
                                      Name = x.Name,
                                      Description = x.Description
                                   } )
            .ToList();
      }

      public static IList<SystemDomainObjectViewModel> SelectSystemDomainObjectViewModels<SourceT>( this IQueryable<SourceT> query )
         where SourceT : SystemDomainObject
      {
         return query.Select( x => new SystemDomainObjectViewModel()
                                   {
                                      Id = x.Id,
                                      Name = x.Name,
                                      Description = x.Description,
                                      AllowUse = (x.StatusCd == 'A'),
                                      IsPredefined = x.IsPredefined
                                   } )
            .ToList();
      }

      public static IList<SelectListItem> SelectSelectListItems<SourceT>( this IQueryable<SourceT> query, Guid selectedId )
         where SourceT : DomainObjectBase
      {
         return query.Select( x => new SelectListItem()
                                   {
                                      Value = x.Id.ToString(),
                                      Text = x.Name,
                                      Selected = (x.Id == selectedId)
                                   } ).ToList();
      }

      public static IList<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<WorkItemType> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItemWithAttributes()
                                      {
                                         Value = item.Id.ToString(),
                                         Text = item.Name,
                                         Selected = item.Id == selectedId,
                                         DataAttributes = new Dictionary<string, string>()
                                                          {
                                                             { "CanBeAssigned", item.IsTask ? "True" : "False" },
                                                             { "CanHaveParent", item.IsTask ? "True" : "False" },
                                                             { "CanHaveChildren", item.IsTask? "False" : "True" }
                                                          }
                                      } ).ToList();
      }

      public static IList<SelectListItem> SelectSelectListItems( this IQueryable<User> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItem()
                                      {
                                         Value = item.Id.ToString(),
                                         Text = (String.IsNullOrWhiteSpace( item.LastName ) ? "" : item.LastName + ", ") + item.FirstName,
                                         Selected = item.Id == selectedId
                                      } ).ToList();
      }

      public static IList<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<WorkItem> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItemWithAttributes()
                                      {
                                         Value = item.Id.ToString(),
                                         Text = item.Name,
                                         Selected = item.Id == selectedId,
                                         DataAttributes = new Dictionary<string, string>()
                                         {
                                            { "ProjectId", item.Project.Id.ToString() },
                                            { "SprintId", (item.Sprint == null ? Guid.Empty : item.Sprint.Id).ToString() }
                                         }
                                      } ).ToList();
      }

      public static IList<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<Sprint> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItemWithAttributes()
         {
            Value = item.Id.ToString(),
            Text = item.Name,
            Selected = item.Id == selectedId,
            DataAttributes = new Dictionary<string, string>()
                                         {
                                            { "ProjectId", item.Project.Id.ToString() },
                                            { "TaskListIsClosed", item.Status.TaskListIsClosed ? "True" : "False" },
                                            { "BacklogIsClosed", item.Status.BacklogIsClosed ? "True" : "False" }
                                         }
         } ).ToList();
      }
   }
}