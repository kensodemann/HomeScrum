using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models
{
   public class EditUserViewModel : UserEditorViewModel
   {
      public EditUserViewModel( User user )
         : base( user ) { }

      public EditUserViewModel()
         : base() { }

      public override bool IsNewUser
      {
         get { return false; }
      }



      public override string Password
      {
         get
         {
            throw new NotImplementedException();
         }
         set
         {
            throw new NotImplementedException();
         }
      }

      public override string ConfirmPassword
      {
         get
         {
            throw new NotImplementedException();
         }
         set
         {
            throw new NotImplementedException();
         }
      }
   }
}