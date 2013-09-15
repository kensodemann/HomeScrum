using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class WorkItemStatusViewModel : Base.SystemDomainObjectViewModel
   {
      [Display( Name = "WorkItemStatusCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual String Category { get; set; }
   }
}