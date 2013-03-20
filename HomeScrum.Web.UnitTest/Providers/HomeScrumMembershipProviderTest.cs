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
   }
}
