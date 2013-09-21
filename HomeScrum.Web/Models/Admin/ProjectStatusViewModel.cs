using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class ProjectStatusViewModel : Base.SystemDomainObjectViewModel
   {
      [Display( Name = "ProjectStatusCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual string Category { get; set; }
   }
}