using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Base;

namespace HomeScrum.Web.Models.Sprints
{
   public class SprintEditorViewModel : DomainObjectViewModel, IEditorViewModel
   {
      [Display( Name = "SprintStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }
      public IEnumerable<SelectListItemWithAttributes> Statuses { get; set; }

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public Guid ProjectId { get; set; }
      public string ProjectName { get; set; }
      public IEnumerable<SelectListItem> Projects { get; set; }

      [Display( Name = "Goal", ResourceType = typeof( DisplayStrings ) )]
      public string Goal { get; set; }

      [Display( Name = "StartDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime? StartDate { get; set; }

      [Display( Name = "EndDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime? EndDate { get; set; }

      public Guid CreatedByUserId { get; set; }
      [Display( Name = "CreatedByUser", ResourceType = typeof( DisplayStrings ) )]
      public string CreatedByUserUserName { get; set; }

      public IEnumerable<SprintWorkItemViewModel> BacklogItems { get; set; }

      public IEnumerable<SprintWorkItemViewModel> Tasks { get; set; }

      public EditMode Mode { get; set; }

      [Display( Name = "Capacity", ResourceType = typeof( DisplayStrings ) )]
      public int Capacity { get; set; }

      [Display( Name = "PointsScheduled", ResourceType = typeof( DisplayStrings ) )]
      public int TotalPoints { get; set; }
   }
}