using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.SqlServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeScrum.Data.UnitTest.Repositories
{
   [TestClass]
   public class UserRepositoryTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext context )
      {
         Database.Initialize();
      }


      [TestInitialize]
      public void InitializeTest()
      {
         Database.Build();
         Users.Load();
         _repository = new UserRepository();
      }

      private IUserRepository _repository;

      [TestMethod]
      public void GetAll_ReturnsAllUseres()
      {
         var statuses = _repository.GetAll();

         Assert.AreEqual( Users.ModelData.GetLength( 0 ), statuses.Count );
         foreach (var status in Users.ModelData)
         {
            AssertCollectionContainsUser( statuses, status );
         }
      }

      [TestMethod]
      public void GetNonExistentUser_ReturnsNull()
      {
         var user = _repository.Get( Guid.NewGuid() );

         Assert.IsNull( user );
      }

      [TestMethod]
      public void GetDefaultId_ReturnsNull()
      {
         var user = _repository.Get( default( Guid ) );

         Assert.IsNull( user );
      }

      [TestMethod]
      public void Get_ReturnsUser()
      {
         var user = _repository.Get( Users.ModelData[2].Id );

         AssertUsersAreEqual( Users.ModelData[2], user );
      }

      [TestMethod]
      public void GetNonExistentUserName_ReturnsNull()
      {
         var user = _repository.Get( "I Do Not Exist" );

         Assert.IsNull( user );
      }

      [TestMethod]
      public void GetEmptyUserName_ReturnsNull()
      {
         var user = _repository.Get( "" );

         Assert.IsNull( user );
      }

      [TestMethod]
      public void GetUserName_ReturnsUser()
      {
         var user = _repository.Get( Users.ModelData[3].UserName );

         AssertUsersAreEqual( Users.ModelData[3], user );
      }

      [TestMethod]
      public void Add_AddsUserToDatabase()
      {
         var user = new User()
         {
            UserName = "new",
            FirstName = "Nathan",
            MiddleName = "Edward",
            LastName = "Wilkes",
            StatusCd = 'A'
         };

         _repository.Add( user );
         Assert.AreEqual( Users.ModelData.GetLength( 0 ) + 1, _repository.GetAll().Count );
         AssertCollectionContainsUser( _repository.GetAll(), user );
      }

      [TestMethod]
      public void Update_ModifiesNameInDatabase()
      {
         var user = Users.ModelData[3];

         user.FirstName += "Modified";

         _repository.Update( user );

         Assert.AreEqual( Users.ModelData.GetLength( 0 ), _repository.GetAll().Count );
         AssertUsersAreEqual( user, _repository.Get( user.Id ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var user = Users.ModelData[2];

         _repository.Delete( user );

         Assert.AreEqual( Users.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.UserName == user.UserName ) );
      }


      private void AssertCollectionContainsUser( ICollection<User> users, User user )
      {
         var userFromCollection = users.FirstOrDefault( x => x.UserName == user.UserName );

         Assert.IsNotNull( userFromCollection );
         AssertUsersAreEqual( user, userFromCollection );
      }

      private static void AssertUsersAreEqual( User expected, User actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.UserName, actual.UserName );
         Assert.AreEqual( expected.FirstName, actual.FirstName );
         Assert.AreEqual( expected.LastName, actual.LastName );
         Assert.AreEqual( expected.MiddleName, actual.MiddleName );
         Assert.AreEqual( expected.StatusCd, actual.StatusCd );
      }
   }
}
