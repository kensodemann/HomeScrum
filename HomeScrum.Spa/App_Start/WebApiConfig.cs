using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HomeScrum.Spa
{
   public static class WebApiConfig
   {
      public static void Register( HttpConfiguration config )
      {
         config.Routes.MapHttpRoute(
             name: "DefaultApi",
             routeTemplate: "api/{controller}/{id}",
             defaults: new { id = RouteParameter.Optional }
         );

         var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault( t => t.MediaType == "application/xml" );
         config.Formatters.XmlFormatter.SupportedMediaTypes.Remove( appXmlType );
         config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
         config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
      }
   }
}
