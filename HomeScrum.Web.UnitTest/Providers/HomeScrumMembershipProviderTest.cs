﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Repositories;
using Moq;
using HomeScrum.Web.Providers;

namespace HomeScrum.Web.UnitTest.Providers
{
   [TestClass]
   public class HomeScrumMembershipProviderTest
   {
      private Mock<ISecurityService> _securityService;
      private HomeScrumMembershipProvider membershipProvider;

      [TestInitialize]
      public void InitializeTest()
      {
         _securityService = new Mock<ISecurityService>();
         membershipProvider = new HomeScrumMembershipProvider( _securityService.Object );
      }

      [TestMethod]
      public void ValidateUserReturnsTrue_IfLoginIsValid()
      {
         _securityService.Setup( x => x.IsValidLogin( "jimmy", "crackcorn" ) )
            .Returns( true )
            .Verifiable();

         var result = membershipProvider.ValidateUser( "jimmy", "crackcorn" );

         _securityService.Verify();
         Assert.IsTrue( result );
      }

      [TestMethod]
      public void ValidateUserReturnsFalse_IfLoginIsNotValid()
      {
         _securityService.Setup( x => x.IsValidLogin( "jimmy", "crackcorn" ) )
            .Returns( false )
            .Verifiable();

         var result = membershipProvider.ValidateUser( "jimmy", "crackcorn" );

         _securityService.Verify();
         Assert.IsFalse( result );
      }

      [TestMethod]
      public void ChangePasswordReturnsTrue_IfPasswordChanged()
      {
         _securityService.Setup( x => x.ChangePassword( "jimmy", "crackcorn", "idontcare" ) )
            .Returns( true )
            .Verifiable();

         var result = membershipProvider.ChangePassword( "jimmy", "crackcorn", "idontcare" );

         _securityService.Verify();
         Assert.IsTrue( result );
      }

      [TestMethod]
      public void ChangePasswordReturnsFalse_IfPasswordNotChanged()
      {
         _securityService.Setup( x => x.ChangePassword( "jimmy", "crackcorn", "idontcare" ) )
            .Returns( false )
            .Verifiable();

         var result = membershipProvider.ChangePassword( "jimmy", "crackcorn", "idontcare" );

         _securityService.Verify();
         Assert.IsFalse( result );
      }

      [TestMethod]
      public void GetUser_ReturnsAUser()
      {
         var result = membershipProvider.GetUser( "jimmy", true );

         Assert.AreEqual( "jimmy", result.UserName );
         Assert.IsNotNull( result );
      }
   }
}
