using HomeScrum.Data.Domain;
using HomeScrum.Web.Models.Admin;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Models.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.Extensions
{
   public static class QueryProjections
   {
      #region Select Lists
      public static IQueryable<SelectListItem> SelectSelectListItems<SourceT>( this IQueryable<SourceT> query, Guid selectedId )
         where SourceT : DomainObjectBase
      {
         return query.Select( x => new SelectListItem()
                                   {
                                      Value = x.Id.ToString(),
                                      Text = x.Name,
                                      Selected = (x.Id == selectedId)
                                   } );
      }

      public static IQueryable<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<SprintStatus> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItemWithAttributes()
         {
            Value = item.Id.ToString(),
            Text = item.Name,
            Selected = item.Id == selectedId,
            DataAttributes = new Dictionary<string, string>()
                                                          {
                                                             { "IsOpenStatus", (item.Category != SprintStatusCategory.Complete) ? "True" : "False" },
                                                             { "TaskListIsClosed", item.TaskListIsClosed ? "True" : "False" },
                                                             { "BacklogIsClosed", item.BacklogIsClosed? "True" : "False" }
                                                          }
         } );
      }

      public static IQueryable<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<WorkItemStatus> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItemWithAttributes()
         {
            Value = item.Id.ToString(),
            Text = item.Name,
            Selected = item.Id == selectedId,
            DataAttributes = new Dictionary<string, string>()
                                                          {
                                                             { "IsOpenStatus", item.Category != WorkItemStatusCategory.Complete ? "True" : "False" },
                                                             { "WorkStarted", item.Category != WorkItemStatusCategory.Unstarted ? "True" : "False" }
                                                          }
         } );
      }

      public static IQueryable<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<WorkItemType> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItemWithAttributes()
                                      {
                                         Value = item.Id.ToString(),
                                         Text = item.Name,
                                         Selected = item.Id == selectedId,
                                         DataAttributes = new Dictionary<string, string>()
                                                          {
                                                             { "CanBeAssigned", item.Category != WorkItemTypeCategory.BacklogItem ? "True" : "False" },
                                                             { "CanHaveParent", item.Category != WorkItemTypeCategory.BacklogItem ? "True" : "False" },
                                                             { "CanHaveChildren", item.Category == WorkItemTypeCategory.BacklogItem ? "True" : "False" }
                                                          }
                                      } );
      }

      public static IQueryable<SelectListItem> SelectSelectListItems( this IQueryable<User> query, Guid selectedId )
      {
         return query.Select( item => new SelectListItem()
                                      {
                                         Value = item.Id.ToString(),
                                         Text = (String.IsNullOrWhiteSpace( item.LastName ) ? "" : item.LastName + ", ") + item.FirstName,
                                         Selected = item.Id == selectedId
                                      } );
      }

      public static IQueryable<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<WorkItem> query, Guid selectedId )
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
                                      } );
      }

      public static IQueryable<SelectListItemWithAttributes> SelectSelectListItems( this IQueryable<Sprint> query, Guid selectedId )
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
         } );
      }
      #endregion


      #region Domain Model to View Model
      public static IQueryable<DomainObjectViewModel> SelectDomainObjectViewModels<SourceT>( this IQueryable<SourceT> query )
         where SourceT : DomainObjectBase
      {
         return query.Select( x => new DomainObjectViewModel()
         {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
         } );
      }

      public static IQueryable<WorkItemIndexViewModel> SelectWorkItemIndexViewModels( this IQueryable<WorkItem> query )
      {
         return query.Select( x => new WorkItemIndexViewModel()
         {
            Id = x.Id,
            Name = x.Name,
            WorkItemTypeName = x.WorkItemType.Name,
            StatusName = x.Status.Name,
            ProjectName = x.Project.Name,
            IsComplete = x.Status.Category == WorkItemStatusCategory.Complete,
            Points = (x.WorkItemType.Category == 0 ? (x.Tasks.Count() == 0 ? 0 : x.Tasks.Sum( t => t.Points )) : x.Points),
            PointsRemaining = (x.WorkItemType.Category == 0 ? (x.Tasks.Count() == 0 ? 0 : x.Tasks.Sum( t => t.PointsRemaining )) : x.PointsRemaining)
         } );
      }
      #endregion
   }
}