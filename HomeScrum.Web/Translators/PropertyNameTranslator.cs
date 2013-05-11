using HomeScrum.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HomeScrum.Web.Translators
{
   public class PropertyNameTranslator<TargetT> : IPropertyNameTranslator<TargetT>
   {
      public PropertyNameTranslator()
      {
         _propertyMap = new Dictionary<string, string>();
      }

      private Dictionary<String, String> _propertyMap;

      public void AddTranslation<T>( Expression<Func<T>> fromProperty, string toPropertyName )
      {
         AddTranslation( ClassHelper.ExtractPropertyName( fromProperty ), toPropertyName );
      }

      public void AddTranslation( string fromPropertyName, string toPropertyName )
      {
         _propertyMap.Add( fromPropertyName, toPropertyName );
      }


      public string TranslatedName<T>( Expression<Func<T>> propertyExpression )
      {
         return TranslatedName( ClassHelper.ExtractPropertyName( propertyExpression ) );
      }

      public string TranslatedName( string propertyName )
      {
         if (_propertyMap.ContainsKey( propertyName ))
         {
            return _propertyMap[propertyName];
         }

         return propertyName;
      }
   }
}