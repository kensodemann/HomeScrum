using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;

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
