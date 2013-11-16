using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace HomeScrum.Web.Models.WorkItems
{
   public class WorkItemEditorViewModel : DomainObjectViewModel, IEditorViewModel
   {
      public WorkItemEditorViewModel()
      {
         this.Tasks = new List<WorkItemIndexViewModel>();
      }

      [Display( Name = "WorkItemStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }
      public IEnumerable<SelectListItemWithAttributes> Statuses { get; set; }

      [Display( Name = "WorkItemType", ResourceType = typeof( DisplayStrings ) )]
      public Guid WorkItemTypeId { get; set; }
      public string WorkItemTypeName { get; set; }
      public IEnumerable<SelectListItemWithAttributes> WorkItemTypes { get; set; }

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public Guid ProjectId { get; set; }
      public string ProjectName { get; set; }
      public IEnumerable<SelectListItem> Projects { get; set; }

      [Display( Name = "Sprint", ResourceType = typeof( DisplayStrings ) )]
      public Guid SprintId { get; set; }
      public string SprintName { get; set; }
      public IEnumerable<SelectListItemWithAttributes> Sprints { get; set; }

      public Guid CreatedByUserId { get; set; }
      [Display( Name = "CreatedByUser", ResourceType = typeof( DisplayStrings ) )]
      public string CreatedByUserUserName { get; set; }

      [Display( Name = "AssignedToUser", ResourceType = typeof( DisplayStrings ) )]
      public Guid AssignedToUserId { get; set; }
      public string AssignedToUserUserName { get; set; }
      public IEnumerable<SelectListItem> AssignedToUsers { get; set; }

      [Display( Name = "ParentWorkItem", ResourceType = typeof( DisplayStrings ) )]
      public Guid ParentWorkItemId { get; set; }
      public string ParentWorkItemName { get; set; }
      public IEnumerable<SelectListItemWithAttributes> ProductBacklogItems { get; set; }

      public IEnumerable<WorkItemIndexViewModel> Tasks { get; set; }


      public void ClearSelectedBacklog()
      {
         var backlogItem = ProductBacklogItems.SingleOrDefault( x => x.Selected );
         if (backlogItem != null)
         {
            backlogItem.Selected = false;
            ParentWorkItemId = Guid.Empty;
         }
      }

      public void ClearSelectedSprint()
      {
         var sprint = Sprints.SingleOrDefault( x => x.Selected );
         if (sprint != null)
         {
            sprint.Selected = false;
            SprintId = Guid.Empty;
         }
      }

      [Display( Name = "Points", ResourceType = typeof( DisplayStrings ) )]
      public int Points { get; set; }

      [Display( Name = "PointsRemaining", ResourceType = typeof( DisplayStrings ) )]
      public int PointsRemaining { get; set; }

      public EditMode Mode { get; set; }
   }
}