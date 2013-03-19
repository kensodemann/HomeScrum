using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.Repositories
{
   public interface ISecurityRepository
   {
      bool IsValidLogin( string userId, string password );
      void ChangePassword( string userId, string oldPassword, string newPassword );
   }
}
