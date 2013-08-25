using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Sprints
{
   public class WorkItemsListForSprintViewModel : Base.DomainObjectViewModel
   {
      public IList<SprintWorkItemViewModel> WorkItems { get; set; }
   }
}