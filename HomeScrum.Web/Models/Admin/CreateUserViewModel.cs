using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public class CreateUserViewModel : UserEditorViewModel
   {
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