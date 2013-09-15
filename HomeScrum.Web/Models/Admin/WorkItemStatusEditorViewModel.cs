﻿using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class WorkItemStatusEditorViewModel : Base.SystemDomainObjectViewModel
   {
      //[Display( Name = "WorkItemStatusIsOpenStatus", ResourceType = typeof( DisplayStrings ) )]
      //public virtual bool IsOpenStatus { get; set; }

      public virtual WorkItemStatusCategory Category { get; set; }
   }
}