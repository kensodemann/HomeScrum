using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Models
{
   public class ProjectEditorViewModel : Project
   {
      public ProjectEditorViewModel()
         : base() { }

      public ProjectEditorViewModel( Project model )
         : base( model ) { }

      public IEnumerable<SelectListItem> ProjectStatuses { get; set; }
   }
}