using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.UnitTest.Helpers
{
   class AttributeHelper
   {
      public static DisplayAttribute GetDisplayAttribute( Type modelType, string propertyName )
      {
         return GetAttribute( modelType, typeof(DisplayAttribute), propertyName ) as DisplayAttribute;
      }

      private static Attribute GetAttribute( Type modelType, Type attributeType, string propertyName )
      {
         var propertyInfo = modelType.GetProperty( propertyName );

         return (propertyInfo != null)
            ? Attribute.GetCustomAttribute( propertyInfo, attributeType )
            : null;
      }
   }
}
