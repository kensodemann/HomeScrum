using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Domain
{
   public class User
   {
      [Required( ErrorMessageResourceName = "UserIdIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      [Display( Name = "UserId", Prompt = "UserIdPrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string UserId { get; set; }

      [Required( ErrorMessageResourceName = "FirstNameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      [Display( Name = "FirstName", Prompt = "FirstNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string FirstName { get; set; }

      [Display( Name = "MiddleName", ResourceType = typeof( DisplayStrings ) )]
      public virtual string MiddleName { get; set; }

      [Display( Name = "LastName", ResourceType = typeof( DisplayStrings ) )]
      public virtual string LastName { get; set; }

      public virtual char StatusCd { get; set; }

      [Display( Name = "UserIsActive", ResourceType = typeof( DisplayStrings ) )]
      public virtual bool IsActive
      {
         get { return StatusCd == 'A'; }
         set { StatusCd = value ? 'A' : 'I'; }
      }
   }
}
