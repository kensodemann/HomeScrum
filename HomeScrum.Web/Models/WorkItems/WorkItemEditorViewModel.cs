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
   }
}