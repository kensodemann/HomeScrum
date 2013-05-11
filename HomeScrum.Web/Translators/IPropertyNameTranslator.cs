using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Web.Translators
{
   public interface IPropertyNameTranslator<TargetT>
   {
      void AddTranslation<T>( Expression<Func<T>> fromProperty, string toPropertyName );
      void AddTranslation( string fromPropertyName, string toPropertyName );

      string TranslatedName<T>( Expression<Func<T>> propertyExpression );
      string TranslatedName( string propertyName );
   }
}
