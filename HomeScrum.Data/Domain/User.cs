using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class User
   {
      public User() { }

      public User( User user )
      {
         this.Id = user.Id;
         this.UserName = user.UserName;
         this.FirstName = user.FirstName;
         this.MiddleName = user.MiddleName;
         this.LastName = user.LastName;
         this.StatusCd = user.StatusCd;
      }
     
      public virtual Guid Id { get; set; }

      [Required( ErrorMessageResourceName = "UserNameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      [Display( Name = "UserName", Prompt = "UserNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string UserName { get; set; }

      [Required( ErrorMessageResourceName = "FirstNameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      [Display( Name = "FirstName", Prompt = "FirstNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string FirstName { get; set; }

      [Display( Name = "MiddleName", ResourceType = typeof( DisplayStrings ) )]
      public virtual string MiddleName { get; set; }

      [Display( Name = "LastName", ResourceType = typeof( DisplayStrings ) )]
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

      [Display( Name = "UserIsActive", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsActive
      {
         get { return StatusCd == 'A'; }
         set { StatusCd = value ? 'A' : 'I'; }
      }

      //public virtual User ShallowCopy()
      //{
      //   return (User)this.MemberwiseClone();
      //}
   }
}
