using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class CreateUserViewModel : UserEditorViewModel
   {
      public CreateUserViewModel()
         : base() { }

      public override bool IsNewUser
      {
         get { return true; }
      }

      // TODO: Get the display strings from HomeScrum.Data.DisplayStrings
      [Required]
      [StringLength( 100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6 )]
      [Display( Name = "Password:" )]
      public override string NewPassword { get; set; }

      [Required]
      [Display( Name = "Confirm password:" )]
      [Compare( "NewPassword", ErrorMessage = "The password and confirmation password do not match." )]
      public override string ConfirmPassword { get; set; }
   }
}