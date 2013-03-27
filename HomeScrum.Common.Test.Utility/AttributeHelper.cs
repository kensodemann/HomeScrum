﻿using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Data.Common.Test.Utility
{
   public class AttributeHelper
   {
      public static CompareAttribute GetCompareAttribute( Type modelType, string propertyName )
      {
         return GetAttribute( modelType, typeof( CompareAttribute ), propertyName ) as CompareAttribute;
      }

      public static DisplayAttribute GetDisplayAttribute( Type modelType, string propertyName )
      {
         return GetAttribute( modelType, typeof( DisplayAttribute ), propertyName ) as DisplayAttribute;
      }

      public static RequiredAttribute GetRequiredAttribute( Type modelType, string propertyName )
      {
         return GetAttribute( modelType, typeof( RequiredAttribute ), propertyName ) as RequiredAttribute;
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
