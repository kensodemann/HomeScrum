using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.Models
{
   public abstract class UserEditorViewModel : User
   {
      public UserEditorViewModel( User user )
         : base( user ) { }

      public UserEditorViewModel()
         : this( new User() ) { }

      public abstract bool IsNewUser { get; }

      [DataType( DataType.Password )]
      public virtual string NewPassword { get; set; }

      [DataType( DataType.Password )]
      public virtual string ConfirmPassword { get; set; }
   }
}