using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.Models.Admin
{
   public class ProjectViewModel : Base.DomainObjectViewModel
   {
      public string StatusName { get; set; }
   }
}