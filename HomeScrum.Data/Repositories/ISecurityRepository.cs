using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Repositories
{
   public interface ISecurityRepository
   {
      bool IsValidLogin( string userName, string password );
      bool ChangePassword( string userName, string oldPassword, string newPassword );
   }
}
