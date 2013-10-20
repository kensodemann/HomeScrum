using System.Collections.Generic;
using System.Web.Mvc;

namespace HomeScrum.Web.Extensions
{
   /// <summary>
   /// Add data-* atttributes to items in a select list.
   /// Used in conjuction with an HtmlHelper extension to put the DataAttributes in data-* attributes
   /// 
   /// Adapted from code found here: http://stackoverflow.com/questions/11285303/store-extra-value-per-item-in-drop-down-list
   /// </summary>
   public class SelectListItemWithAttributes : SelectListItem
   {
      public IDictionary<string, string> DataAttributes { get; set; }
   }
}