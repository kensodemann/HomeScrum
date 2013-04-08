using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.Models
{
   public class ProjectViewModel : Base.DomainObjectViewModel
   {
      public string ProjectStatusName { get; set; }
   }
}