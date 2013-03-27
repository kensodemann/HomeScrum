using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using HomeScrum.Common.Utility;

namespace HomeScrum.Data.Common.Test.Utility
{
   public class AttributeHelper
   {
      public static CompareAttribute GetCompareAttribute<T>( Expression<Func<T>> propertyExpression )
      {
         return GetAttribute( typeof( CompareAttribute ), propertyExpression ) as CompareAttribute;
      }

      public static DisplayAttribute GetDisplayAttribute<T>( Expression<Func<T>> propertyExpression )
      {
         return GetAttribute( typeof( DisplayAttribute ), propertyExpression ) as DisplayAttribute;
      }

      public static RequiredAttribute GetRequiredAttribute<T>( Expression<Func<T>> propertyExpression )
      {
         return GetAttribute( typeof( RequiredAttribute ), propertyExpression ) as RequiredAttribute;
      }

      private static Attribute GetAttribute<T>( Type attributeType, Expression<Func<T>> propertyExpression )
      {
         var type = ClassHelper.ExtractClassType( propertyExpression );
         var propertyName = ClassHelper.ExtractPropertyName( propertyExpression );
         var propertyInfo = type.GetProperty( propertyName );

         return (propertyInfo != null)
            ? Attribute.GetCustomAttribute( propertyInfo, attributeType )
            : null;
      }
   }
}
