using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Validators
{
   public interface IValidator<T> //where T : DataObjectBase
   {
      bool ModelIsValid( T model );
      ICollection<KeyValuePair<string, string>> Messages { get; }
   }
}
