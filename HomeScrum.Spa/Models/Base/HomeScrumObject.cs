using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Spa.Models.Base
{
   public class HomeScrumObject
   {
      public Guid Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
   }
}