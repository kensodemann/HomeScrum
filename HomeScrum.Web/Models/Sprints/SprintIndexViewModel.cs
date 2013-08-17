using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Sprints
{
   public class SprintIndexViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "SprintStatus", ResourceType = typeof( DisplayStrings ) )]
      public string StatusName { get; set; }

      [Display( Name = "Project", ResourceType = typeof( DisplayStrings ) )]
      public string ProjectName { get; set; }

      [Display( Name = "StartDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime StartDate { get; set; }

      [Display( Name = "EndDate", ResourceType = typeof( DisplayStrings ) )]
      public DateTime EndDate { get; set; }
   }
}