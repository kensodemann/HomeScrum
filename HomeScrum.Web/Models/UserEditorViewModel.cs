using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.Models
{
   public abstract class UserEditorViewModel
   {
      public virtual Guid Id { get; set; }

      [Display( Name = "UserName", Prompt = "UserNamePrompt", ResourceType = typeof( DisplayStrings ) )]
      public virtual string UserName { get; set; }

      [Display( Name = "FirstName", Prompt = "FirstNamePrompt", ResourceType = typeof( DisplayStrings ) )]
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
   }
}