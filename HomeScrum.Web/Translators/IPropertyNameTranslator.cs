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
      string TranslatedName<T>( Expression<Func<T>> propertyExpression );
      string TranslatedName( string propertyName );
   }
}
