using HomeScrum.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeScrum.Web.Models.Admin
{
   public class EditUserViewModel : UserEditorViewModel
   {
      public override bool IsNewUser
      {
         get { return false; }
      }
   }
}