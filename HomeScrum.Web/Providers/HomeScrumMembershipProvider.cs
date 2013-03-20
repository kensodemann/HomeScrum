using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeScrum.Data.Repositories;
using Ninject;
using WebMatrix.WebData;

namespace HomeScrum.Web.Providers
{
   public class HomeScrumMembershipProvider : ExtendedMembershipProvider
   {
      public HomeScrumMembershipProvider() :
         this( DependencyResolver.Current.GetService<ISecurityRepository>() ) { }

      public HomeScrumMembershipProvider( ISecurityRepository securityRepository )
      {
         _securityRepository = securityRepository;
      }


      private ISecurityRepository _securityRepository;


      public override bool ValidateUser( string username, string password )
      {
         return _securityRepository.IsValidLogin( username, password );
      }

      public override bool ChangePassword( string username, string oldPassword, string newPassword )
      {
         return _securityRepository.ChangePassword( username, oldPassword, newPassword );
      }

      // NOTE: This is required by WebSecurity.ChangePassword().  We are not really taking advantage of
      //       any of this structure right now
      //
      // TODO: Look into adding more of this functionallity.  It may also be wise to split this into
      //       a seperate table in the database.
      public override System.Web.Security.MembershipUser GetUser( string username, bool userIsOnline )
      {
         return new System.Web.Security.MembershipUser( "HomeScrumMembershipProvider", username, username, null, null, null, true, false,
            DateTime.MinValue, DateTime.MinValue, DateTime.Now, DateTime.MinValue, DateTime.MinValue );
      }


      #region Unused Methods
      public override bool ConfirmAccount( string accountConfirmationToken )
      {
         throw new NotImplementedException();
      }

      public override bool ConfirmAccount( string userName, string accountConfirmationToken )
      {
         throw new NotImplementedException();
      }

      public override string CreateAccount( string userName, string password, bool requireConfirmationToken )
      {
         throw new NotImplementedException();
      }

      public override string CreateUserAndAccount( string userName, string password, bool requireConfirmation, IDictionary<string, object> values )
      {
         throw new NotImplementedException();
      }

      public override bool DeleteAccount( string userName )
      {
         throw new NotImplementedException();
      }

      public override string GeneratePasswordResetToken( string userName, int tokenExpirationInMinutesFromNow )
      {
         throw new NotImplementedException();
      }

      public override ICollection<OAuthAccountData> GetAccountsForUser( string userName )
      {
         throw new NotImplementedException();
      }

      public override DateTime GetCreateDate( string userName )
      {
         throw new NotImplementedException();
      }

      public override DateTime GetLastPasswordFailureDate( string userName )
      {
         throw new NotImplementedException();
      }

      public override DateTime GetPasswordChangedDate( string userName )
      {
         throw new NotImplementedException();
      }

      public override int GetPasswordFailuresSinceLastSuccess( string userName )
      {
         throw new NotImplementedException();
      }

      public override int GetUserIdFromPasswordResetToken( string token )
      {
         throw new NotImplementedException();
      }

      public override bool IsConfirmed( string userName )
      {
         throw new NotImplementedException();
      }

      public override bool ResetPasswordWithToken( string token, string newPassword )
      {
         throw new NotImplementedException();
      }

      public override string ApplicationName
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

      public override bool ChangePasswordQuestionAndAnswer( string username, string password, string newPasswordQuestion, string newPasswordAnswer )
      {
         throw new NotImplementedException();
      }

      public override System.Web.Security.MembershipUser CreateUser( string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status )
      {
         throw new NotImplementedException();
      }

      public override bool DeleteUser( string username, bool deleteAllRelatedData )
      {
         throw new NotImplementedException();
      }

      public override bool EnablePasswordReset
      {
         get { throw new NotImplementedException(); }
      }

      public override bool EnablePasswordRetrieval
      {
         get { throw new NotImplementedException(); }
      }

      public override System.Web.Security.MembershipUserCollection FindUsersByEmail( string emailToMatch, int pageIndex, int pageSize, out int totalRecords )
      {
         throw new NotImplementedException();
      }

      public override System.Web.Security.MembershipUserCollection FindUsersByName( string usernameToMatch, int pageIndex, int pageSize, out int totalRecords )
      {
         throw new NotImplementedException();
      }

      public override System.Web.Security.MembershipUserCollection GetAllUsers( int pageIndex, int pageSize, out int totalRecords )
      {
         throw new NotImplementedException();
      }

      public override int GetNumberOfUsersOnline()
      {
         throw new NotImplementedException();
      }

      public override string GetPassword( string username, string answer )
      {
         throw new NotImplementedException();
      }

      public override System.Web.Security.MembershipUser GetUser( object providerUserKey, bool userIsOnline )
      {
         throw new NotImplementedException();
      }

      public override string GetUserNameByEmail( string email )
      {
         throw new NotImplementedException();
      }

      public override int MaxInvalidPasswordAttempts
      {
         get { throw new NotImplementedException(); }
      }

      public override int MinRequiredNonAlphanumericCharacters
      {
         get { throw new NotImplementedException(); }
      }

      public override int MinRequiredPasswordLength
      {
         get { throw new NotImplementedException(); }
      }

      public override int PasswordAttemptWindow
      {
         get { throw new NotImplementedException(); }
      }

      public override System.Web.Security.MembershipPasswordFormat PasswordFormat
      {
         get { throw new NotImplementedException(); }
      }

      public override string PasswordStrengthRegularExpression
      {
         get { throw new NotImplementedException(); }
      }

      public override bool RequiresQuestionAndAnswer
      {
         get { throw new NotImplementedException(); }
      }

      public override bool RequiresUniqueEmail
      {
         get { throw new NotImplementedException(); }
      }

      public override string ResetPassword( string username, string answer )
      {
         throw new NotImplementedException();
      }

      public override bool UnlockUser( string userName )
      {
         throw new NotImplementedException();
      }

      public override void UpdateUser( System.Web.Security.MembershipUser user )
      {
         throw new NotImplementedException();
      }
      #endregion
   }
}