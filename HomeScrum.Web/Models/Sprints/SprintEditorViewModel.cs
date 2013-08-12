using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Models.Sprints
{
   public class SprintEditorViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "SprintStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }
      public IEnumerable<SelectListItem> Statuses { get; set; }

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public Guid ProjectId { get; set; }
      public string ProjectName { get; set; }
      public IEnumerable<SelectListItem> Projects { get; set; }

      [Display( Name = "StartDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime StartDate { get; set; }

      [Display( Name = "EndDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime EndDate { get; set; }
   }
}