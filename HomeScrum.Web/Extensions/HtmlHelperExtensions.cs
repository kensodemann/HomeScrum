using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Web.Extensions
{
   public static class HtmlHelperExtensions
   {
      public static MvcHtmlString DisplayFormattedText( this HtmlHelper htmlHelper, string text )
      {
         if (text == null)
         {
            return null;
         }

         var result = htmlHelper.Encode( text );
         result = result.Replace( System.Environment.NewLine, "<br/>" );
         return new MvcHtmlString( result );
      }
   }
}