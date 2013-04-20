using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.WorkItems
{
   public class WorkItemViewModel : Base.DomainObjectViewModel
   {
      public string WorkItemTypeName { get; set; }

      public string StatusName { get; set; }

      public string ProjectName { get; set; }

      //public string CreatedByUserName { get; set; }

      //public string AssignedToUserName { get; set; }
   }
}