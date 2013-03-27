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
       public static string ExtractPropertyName<T>( Expression<Func<T>> propertyExpression )
       {
          var memberExpression = propertyExpression.Body as MemberExpression;
          if (memberExpression == null)
          {
             throw new ArgumentException( "Expression does not access a property", "propertyExpression" );
          }

          var propertyInfo = memberExpression.Member as PropertyInfo;
          if (propertyInfo == null)
          {
             throw new ArgumentException( "Expression does not access a property", "propertyExpression" );
          }

          return propertyInfo.Name;
       }
    }
}
