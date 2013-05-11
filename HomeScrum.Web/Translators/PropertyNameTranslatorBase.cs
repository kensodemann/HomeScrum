using HomeScrum.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HomeScrum.Web.Translators
{
   public class PropertyNameTranslatorBase
   {
      public PropertyNameTranslatorBase()
      {
         _propertyMap = new Dictionary<string, string>();
      }

      private Dictionary<String, String> _propertyMap;

      public string ViewModelPropertyName( string modelPropertyName )
      {
         if (_propertyMap.ContainsKey( modelPropertyName ))
         {
            return _propertyMap[modelPropertyName];
         }

         return modelPropertyName;
      }

      public string ViewModelPropertyName<T>( Expression<Func<T>> expression )
      {
         return ViewModelPropertyName( ClassHelper.ExtractPropertyName( expression ) );
      }
   }
}