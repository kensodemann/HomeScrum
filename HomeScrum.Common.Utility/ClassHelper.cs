using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Common.Utility
{
   public static class ClassHelper
   {
      public static string ExtractPropertyName<T>( Expression<Func<T>> expression )
      {
         var memberExpression = expression.Body as MemberExpression;
         if (memberExpression == null)
         {
            throw new ArgumentException( "Expression does not access a property", "expression" );
         }

         var propertyInfo = memberExpression.Member as PropertyInfo;
         if (propertyInfo == null)
         {
            throw new ArgumentException( "Expression does not access a property", "expression" );
         }

         return propertyInfo.Name;
      }

      public static string ExtractMethodName<T>( Expression<Func<T>> expression )
      {
         var methodExpression = expression.Body as MethodCallExpression;
         if (methodExpression == null)
         {
            throw new ArgumentException( "Expression does not access a method", "expression" );
         }

         return methodExpression.Method.Name;
      }
   }
}
