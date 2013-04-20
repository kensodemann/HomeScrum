using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Models.Admin
{
   public class ProjectEditorViewModel : Base.DomainObjectViewModel
   {
      public Guid LastModifiedUserId { get; set; }

      [Display( Name = "ProjectStatus", ResourceType = typeof( DisplayStrings ) )]
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }

      public IEnumerable<SelectListItem> ProjectStatuses { get; set; }
   }
}