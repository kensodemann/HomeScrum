using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Web.Providers
{
   public interface IWebSecurity
   {
      bool Login( string userName, string password, bool persistCookie = false );
      void Logout();
      bool ChangePassword( string userName, string currentPassword, string newPassword );
      
      IPrincipal CurrentUser { get; }
   }
}
