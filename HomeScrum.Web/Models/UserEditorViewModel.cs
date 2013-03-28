using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.Models
{
   public abstract class UserEditorViewModel : IViewModel<User>
   {
      public UserEditorViewModel( User user )
      {
         DomainModel = user;
      }

      public UserEditorViewModel()
         : this( new User() ) { }


      public User DomainModel { get; set; }

      public abstract bool IsNewUser { get; }

      [DataType( DataType.Password )]
      public virtual string Password { get; set; }

      [DataType( DataType.Password )]
      public virtual string ConfirmPassword { get; set; }
   }
}