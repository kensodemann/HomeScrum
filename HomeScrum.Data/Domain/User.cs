using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Validation;

namespace HomeScrum.Data.Domain
{
   public class User : ValidatableObject
   {
      public virtual Guid Id { get; set; }

      [Required( ErrorMessageResourceName = "UserNameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      public virtual string UserName { get; set; }

      [Required( ErrorMessageResourceName = "FirstNameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      public virtual string FirstName { get; set; }

      public virtual string MiddleName { get; set; }

      public virtual string LastName { get; set; }

      public virtual char StatusCd { get; set; }

      private Byte[] _password;
      protected internal virtual Byte[] Password
      {
         get
         {
            var sha1 = SHA1.Create();
            return sha1.ComputeHash( Encoding.Default.GetBytes( "bogus" ) );
         }
         set { _password = value; }
      }
   }
}
