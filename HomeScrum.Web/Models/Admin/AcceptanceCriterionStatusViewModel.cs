﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class AcceptanceCriterionStatusViewModel : Base.SystemDomainObjectViewModel
   {
      [Display( Name = "AcceptanceCriterionStatusCategory", ResourceType = typeof( DisplayStrings ) )]
      public virtual string Category { get; set; }
   }
}