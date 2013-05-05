using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public class ProjectViewModel : Base.DomainObjectViewModel
   {
      [Display( Name = "ProjectStatus", ResourceType = typeof( DisplayStrings ) )]
      public string StatusName { get; set; }
   }
}