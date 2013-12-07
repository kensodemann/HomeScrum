using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof( HomeScrum.Spa.App_Start.DurandalRouteConfig ), "RegisterDurandalPreStart", Order = 2 )]

namespace HomeScrum.Spa.App_Start
{
   public static class DurandalRouteConfig
   {
      public static void RegisterDurandalPreStart()
      {

         // Preempt standard default MVC page routing to go to HotTowel Sample
         System.Web.Routing.RouteTable.Routes.MapRoute(
             name: "DurandalMvc",
             url: "{controller}/{action}/{id}",
             defaults: new
             {
                controller = "Durandal",
                action = "Index",
                id = UrlParameter.Optional
             }
         );
      }
   }
}