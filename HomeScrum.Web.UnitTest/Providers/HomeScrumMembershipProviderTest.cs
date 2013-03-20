using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Repositories;
using Moq;
using HomeScrum.Web.Providers;

namespace HomeScrum.Web.UnitTest.Providers
{
   [TestClass]
   public class HomeScrumMembershipProviderTest
   {
      private Mock<ISecurityRepository> _securityRepository;
      private HomeScrumMembershipProvider membershipProvider;

      [TestInitialize]
      public void InitializeTest()
      {
         _securityRepository = new Mock<ISecurityRepository>();
         membershipProvider = new HomeScrumMembershipProvider( _securityRepository.Object );
      }

      [TestMethod]
      public void ValidateUserReturnsTrue_IfLoginIsValid()
      {
         _securityRepository.Setup( x => x.IsValidLogin( "jimmy", "crackcorn" ) )
            .Returns( true )
            .Verifiable();

         var result = membershipProvider.ValidateUser( "jimmy", "crackcorn" );

         _securityRepository.Verify();
         Assert.IsTrue( result );
      }

      [TestMethod]
      public void ValidateUserReturnsFalse_IfLoginIsNotValid()
      {
         _securityRepository.Setup( x => x.IsValidLogin( "jimmy", "crackcorn" ) )
            .Returns( false )
            .Verifiable();

         var result = membershipProvider.ValidateUser( "jimmy", "crackcorn" );

         _securityRepository.Verify();
         Assert.IsFalse( result );
      }

      [TestMethod]
      public void ChangePasswordReturnsTrue_IfPasswordChanged()
      {
         _securityRepository.Setup( x => x.ChangePassword( "jimmy", "crackcorn", "idontcare" ) )
            .Returns( true )
            .Verifiable();

         var result = membershipProvider.ChangePassword( "jimmy", "crackcorn", "idontcare" );

         _securityRepository.Verify();
         Assert.IsTrue( result );
      }

      [TestMethod]
      public void ChangePasswordReturnsFalse_IfPasswordNotChanged()
      {
         _securityRepository.Setup( x => x.ChangePassword( "jimmy", "crackcorn", "idontcare" ) )
            .Returns( false )
            .Verifiable();

         var result = membershipProvider.ChangePassword( "jimmy", "crackcorn", "idontcare" );

         _securityRepository.Verify();
         Assert.IsFalse( result );
      }
   }
}
