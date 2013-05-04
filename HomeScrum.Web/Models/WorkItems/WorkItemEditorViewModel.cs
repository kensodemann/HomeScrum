using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Models.WorkItems
{
   public class WorkItemEditorViewModel : Base.DomainObjectViewModel
   {
      public Guid StatusId { get; set; }
      public string StatusName { get; set; }
      public IEnumerable<SelectListItem> Statuses { get; set; }

      public Guid WorkItemTypeId { get; set; }
      public string WorkItemTypeName { get; set; }
      public IEnumerable<SelectListItem> WorkItemTypes { get; set; }

      public Guid ProjectId { get; set; }
      public string ProjectName { get; set; }
      public IEnumerable<SelectListItem> Projects { get; set; }

      public Guid CreatedByUserId { get; set; }
      public string CreatedByUserUserName { get; set; }

      public Guid AssignedToUserId { get; set; }
      public string AssignedToUserUserName { get; set; }
      public IEnumerable<SelectListItem> AssignedToUsers { get; set; }
   }
}