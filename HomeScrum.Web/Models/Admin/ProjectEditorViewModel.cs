using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Models.Admin
{
   public class ProjectEditorViewModel : Base.DomainObjectEditorViewModel
   {
      public Guid LastModifiedUserId { get; set; }

      [Display( Name = "ProjectStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid ProjectStatusId { get; set; }
      public string ProjectStatusName { get; set; }

      public IEnumerable<SelectListItem> ProjectStatuses { get; set; }
   }
}