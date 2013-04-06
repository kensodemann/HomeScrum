using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Base
{
   public class SystemDomainObjectViewModel : DomainObjectViewModel
   {
      [Display( Name = "AllowUse", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool AllowUse { get; set; }

      public virtual bool IsPredefined { get; set; }
   }
}