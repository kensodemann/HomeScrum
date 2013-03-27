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
      public UserEditorViewModel( User user )
      {
         User = user;
      }

      public UserEditorViewModel()
         : this( new User() ) { }


      public User User { get; set; }

      public abstract bool IsNewUser { get; }

      [DataType( DataType.Password )]
      public abstract string Password { get; set; }

      [DataType( DataType.Password )]
      public abstract string ConfirmPassword { get; set; }
   }
}