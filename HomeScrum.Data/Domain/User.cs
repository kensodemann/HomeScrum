using HomeScrum.Data.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace HomeScrum.Data.Domain
{
   public class User : ValidatableObject
   {
      public User()
         : base( null ) { }

      public virtual Guid Id { get; set; }

      [Required]
      public virtual string UserName { get; set; }

      [Required]
      public virtual string FirstName { get; set; }

      public virtual string MiddleName { get; set; }

      public virtual string LastName { get; set; }

      public virtual char StatusCd { get; set; }

      private Byte[] _password;
      protected internal virtual Byte[] Password
      {
         get
         {
            return _password;
         }
         set { _password = value; }
      }

      public virtual void SetPassword( string rawPassword )
      {
         var sha1 = SHA1.Create();
         Password = sha1.ComputeHash( Encoding.Default.GetBytes(rawPassword ) );
      }
   }
}
