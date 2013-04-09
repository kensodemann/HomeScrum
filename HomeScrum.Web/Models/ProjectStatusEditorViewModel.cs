﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class ProjectStatusEditorViewModel : Base.SystemDomainObjectEditorViewModel
   {
      [Display( Name = "ProjectStatusIsActive", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsActive { get; set; }
   }
}