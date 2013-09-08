﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using HomeScrum.Web.Extensions;

namespace HomeScrum.Web.Models.Sprints
{
   public class SprintEditorViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "SprintStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }
      public IEnumerable<SelectListItemWithAttributes> Statuses { get; set; }

      // Any select list that can be disabled needs two Id properties.  One to use for the
      // select list, and the actual one which is used in a hidden element.  This is done
      // because disabled elements are not included in the form submission.

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public Guid ProjectId { get; set; }
      public Guid SelectProjectId
      {
         get { return ProjectId; }
         set { ;}
      }
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
   }
}