using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Binders
{
   public class PrincipalModelBinder : IModelBinder
   {
      public object BindModel( ControllerContext controllerContext, ModelBindingContext bindingContext )
      {
         if (controllerContext == null)
         {
            throw new ArgumentNullException( "controllerContext" );
         }
         if (bindingContext == null)
         {
            throw new ArgumentNullException( "bindingContext" );
         }
         return HttpContext.Current.User;
      }
   }
}