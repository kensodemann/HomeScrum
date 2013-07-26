using HomeScrum.Web.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Models.WorkItems
{
   public class WorkItemEditorViewModel : Base.DomainObjectViewModel
   {
      public WorkItemEditorViewModel()
      {
         this.Tasks = new List<WorkItemIndexViewModel>();
      }



      [Display( Name = "WorkItemStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }
      public IEnumerable<SelectListItem> Statuses { get; set; }

      [Display( Name = "WorkItemType", ResourceType = typeof( DisplayStrings ) )]
      public Guid WorkItemTypeId { get; set; }
      public string WorkItemTypeName { get; set; }
      public IEnumerable<SelectListItemWithAttributes> WorkItemTypes { get; set; }

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public Guid ProjectId { get; set; }
      public string ProjectName { get; set; }
      public IEnumerable<SelectListItem> Projects { get; set; }

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

      public string CallingAction { get; set; }
      public Guid CallingId { get; set; }
   }
}