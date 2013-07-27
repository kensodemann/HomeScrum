using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Base
{
   public class ViewModelBase
   {
      public string CallingAction { get; set; }
      public Guid CallingId { get; set; }
   }
}