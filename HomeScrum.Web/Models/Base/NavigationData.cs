using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Base
{
   [Serializable]
   public class NavigationData
   {
      public string Controller;
      public string Action;
      public string Id;
   }
}