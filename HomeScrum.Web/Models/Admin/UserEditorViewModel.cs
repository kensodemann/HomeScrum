using HomeScrum.Web.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public abstract class UserEditorViewModel : ViewModelBase, IEditorViewModel
   {
      public Guid Id { get; set; }

      [Display( Name = "UserName", Prompt = "UserNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      [Required( ErrorMessageResourceName = "UserNameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      public virtual string UserName { get; set; }

      [Display( Name = "FirstName", Prompt = "FirstNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      [Required( ErrorMessageResourceName = "FirstNameIsRequired", ErrorMessageResourceType = typeof( ErrorMessages ) )]
      public virtual string FirstName { get; set; }

      [Display( Name = "MiddleName", ResourceType = typeof( DisplayStrings ) )]
      public virtual string MiddleName { get; set; }

      [Display( Name = "LastName", ResourceType = typeof( DisplayStrings ) )]
      public virtual string LastName { get; set; }

      public virtual bool IsActive { get; set; }

      public abstract bool IsNewUser { get; }

      [DataType( DataType.Password )]
      public virtual string NewPassword { get; set; }

      [DataType( DataType.Password )]
      public virtual string ConfirmPassword { get; set; }

      public EditMode Mode { get; set; }
   }
}