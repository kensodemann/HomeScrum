using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace HomeScrum.Web.Providers
{
   public class WebSecurityWrapper:IWebSecurity
   {
      public bool Login( string userName, string password, bool persistCookie = false )
      {
         return WebSecurity.Login( userName, password, persistCookie );
      }

      public void Logout()
      {
         WebSecurity.Logout();
      }

      public bool ChangePassword( string userName, string currentPassword, string newPassword )
      {
         return WebSecurity.ChangePassword( userName, currentPassword, newPassword );
      }

      public System.Security.Principal.IPrincipal CurrentUser
      {
         get { return HttpContext.Current.User; }
      }
   }
}