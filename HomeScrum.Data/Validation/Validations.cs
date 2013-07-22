using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using NHibernate.Linq;
using NHibernate;

namespace HomeScrum.Data.Validation
{
   internal static class Validations
   {
      public static void VerifyNameIsUnique<T>( this T model, ISession session )
      where T : DomainObjectBase
      {
         if (session.Query<T>()
                .Where( x => x.Id != model.Id && x.Name == model.Name )
                .ToList().Count > 0)
         {
            model.AddErrorMessage( "Name", String.Format( ErrorMessages.NameIsNotUnique, model.GetObjectName(), model.Name ) );
         }
      }
   }
}
