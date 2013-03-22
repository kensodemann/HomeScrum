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
         _repository = new Repository<User, String>();
      }

      private IRepository<User, String> _repository;

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
         var user = _repository.Get( "IDontExist" );

         Assert.IsNull( user );
      }

      [TestMethod]
      public void GetNullUserId_ReturnsNull()
      {
         var user = _repository.Get( null );

         Assert.IsNull( user );
      }

      [TestMethod]
      public void Get_ReturnsUser()
      {
         var user = _repository.Get( Users.ModelData[2].UserId );

         AssertUsersAreEqual( Users.ModelData[2], user );
      }

      [TestMethod]
      public void Add_AddsUserToDatabase()
      {
         var user = new User()
         {
            UserId = "new",
            FirstName ="Nathan",
            MiddleName = "Edward",
            LastName = "Wilkes",
            IsActive = true
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
         AssertUsersAreEqual( user, _repository.Get( user.UserId ) );
      }

      [TestMethod]
      public void Delete_RevmovesItemFromDatabase()
      {
         var user = Users.ModelData[2];

         _repository.Delete( user );

         Assert.AreEqual( Users.ModelData.GetLength( 0 ) - 1, _repository.GetAll().Count );
         Assert.IsNull( _repository.GetAll().FirstOrDefault( x => x.UserId == user.UserId ) );
      }


      private void AssertCollectionContainsUser( ICollection<User> users, User user )
      {
         var userFromCollection = users.FirstOrDefault( x => x.UserId == user.UserId );

         Assert.IsNotNull( userFromCollection );
         AssertUsersAreEqual( user, userFromCollection );
      }

      private static void AssertUsersAreEqual( User expected, User actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.UserId, actual.UserId );
         Assert.AreEqual( expected.FirstName, actual.FirstName );
         Assert.AreEqual( expected.LastName, actual.LastName );
         Assert.AreEqual( expected.MiddleName, actual.MiddleName );
         Assert.AreEqual( expected.IsActive, actual.IsActive );
      }
   }
}
