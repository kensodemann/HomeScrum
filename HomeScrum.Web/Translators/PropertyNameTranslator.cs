using HomeScrum.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace HomeScrum.Web.Translators
{
   public class PropertyNameTranslator<SourceT, TargetT> : IPropertyNameTranslator<SourceT, TargetT>
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
         return ExplicitTranslation( propertyName ) ??
            ConventionBasedMapping( propertyName ) ??
            propertyName;
      }

      private string ExplicitTranslation( string propertyName )
      {
         if (_propertyMap.ContainsKey( propertyName ))
         {
            return _propertyMap[propertyName];
         }

         return null;
      }

      private string ConventionBasedMapping( string propertyName )
      {
         var conventionPropertyName = propertyName + "Id";
         if (SourcePropertyHasId( propertyName ) && TypeHasProperty( typeof( TargetT ), conventionPropertyName ))
         {
            return conventionPropertyName;
         }

         return null;
      }

      private bool SourcePropertyHasId( string propertyName )
      {
         var sourceType = typeof( SourceT );
         var property = sourceType.GetProperty( propertyName );

         return TypeHasProperty( property.PropertyType, "Id" );
      }

      private bool TypeHasProperty( Type theType, string propertyName )
      {
         var property = theType.GetProperty( propertyName );

         return property != null;
      }
   }
}