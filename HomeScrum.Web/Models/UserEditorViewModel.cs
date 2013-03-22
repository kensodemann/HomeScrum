using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Data.Domain;

namespace HomeScrum.Web.Models
{
   public class UserEditorViewModel
   {
      public UserEditorViewModel( User user )
      {
         User = user;
         Password = new LocalPasswordModel();
      }

      public UserEditorViewModel()
         : this( new User() ) { }


      public User User { get; set; }
      public LocalPasswordModel Password { get; set; }
   }
}