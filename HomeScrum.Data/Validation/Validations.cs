using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Common.Utility;
using HomeScrum.Data.Domain;
using NHibernate.Linq;

namespace HomeScrum.Data.Validation
{
   internal static class Validations
   {
      public static void VerifyNameIsUnique<T>( this T model )
      where T : DomainObjectBase
      {
         using (var session = NHibernateHelper.OpenSession())
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
}
