using HomeScrum.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Xml.Linq;

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

      // Adapted from code found here: http://stackoverflow.com/questions/11285303/store-extra-value-per-item-in-drop-down-list
      public static MvcHtmlString DropDownListWithDataAttributesFor<TModel, TValue>( this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItemWithAttributes> selectList, object htmlAttributes = null )
      {
         string name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName( ExpressionHelper.GetExpressionText( expression ) );

         var selectDoc = XDocument.Parse( htmlHelper.DropDownList( name, (IEnumerable<SelectListItem>)selectList, htmlAttributes ).ToString() );

         var options = from XElement el in selectDoc.Element( "select" ).Descendants()
                       select el;

         for (int i = 0; i < options.Count(); i++)
         {
            var option = options.ElementAt( i );
            var attributes = selectList.ElementAt( i );

            foreach (var attribute in attributes.DataAttributes)
            {
               option.SetAttributeValue( "data-" + attribute.Key, attribute.Value );
            }
         }

         selectDoc.Root.ReplaceNodes( options.ToArray() );
         return MvcHtmlString.Create( selectDoc.ToString() );
      }

      // Adapted from code found in a couple of places:
      //     http://stackoverflow.com/questions/388483/how-do-you-create-a-dropdownlist-from-an-enum-in-asp-net-mvc
      //     http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx
      public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>( this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes = null )
      {
         ModelMetadata metadata = ModelMetadata.FromLambdaExpression( expression, htmlHelper.ViewData );
         IEnumerable<TEnum> values = Enum.GetValues( typeof( TEnum ) ).Cast<TEnum>();

         var items = values.Select( x => new { Id = x, Name = EnumHelper.GetDescription( x ) } );
         var selectList = new SelectList( items, "Id", "Name", metadata.Model );

         return htmlHelper.DropDownListFor( expression, selectList, htmlAttributes );
      }
   }
}