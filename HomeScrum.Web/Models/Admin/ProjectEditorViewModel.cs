using HomeScrum.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HomeScrum.Web.Models.Admin
{
   public class ProjectEditorViewModel : DomainObjectViewModel, IEditorViewModel
   {
      public Guid LastModifiedUserId { get; set; }

      [Display( Name = "ProjectStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }
      public IEnumerable<SelectListItem> Statuses { get; set; }

      public EditMode Mode { get; set; }
   }
}