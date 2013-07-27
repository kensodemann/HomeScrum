using HomeScrum.Web.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace HomeScrum.Web.Models.Admin
{
   public class UserViewModel : ViewModelBase
   {
      public Guid Id { get; set; }

      [Display( Name = "UserName", Prompt = "UserNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public string UserName { get; set; }

      [Display( Name = "FirstName", Prompt = "FirstNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public string FirstName { get; set; }

      [Display( Name = "MiddleName", ResourceType = typeof( DisplayStrings ) )]
      public string MiddleName { get; set; }

      [Display( Name = "LastName", ResourceType = typeof( DisplayStrings ) )]
      public string LastName { get; set; }

      [Display( Name = "UserIsActive", ResourceType = typeof( DisplayStrings ) )]
      public bool IsActive { get; set; }
   }
}