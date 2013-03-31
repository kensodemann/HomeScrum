using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Domain;
using Moq;
using HomeScrum.Data.Validators;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class UserValidatorTest
   {
      private Mock<IRepository<User, Guid>> _userRepository;
      private UserValidator _validator;

      private User CreateNewUser()
      {
         return new User()
         {
            Id = Guid.NewGuid(),
            UserName = "Unique",
            FirstName = "I",
            MiddleName = "Am",
            LastName = "Unique",
            StatusCd = 'A',
            IsActive = true
         };
      }

      private User CopyUser( User user )
      {
         return new User()
         {
            Id = user.Id,
            UserName = user.UserName,
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            StatusCd = user.StatusCd
         };
      }

      [TestInitialize]
      public void InitializeTest()
      {
         _userRepository = new Mock<IRepository<User, Guid>>();
         Users.Load();
         _userRepository
            .Setup( x => x.GetAll() )
            .Returns( Users.ModelData );

         _validator = new UserValidator( _userRepository.Object );
      }

      [TestMethod]
      public void ModelIsValidOnInsert_IfUserNameIsUnique()
      {
         var user = CreateNewUser();

         var result = _validator.ModelIsValid( user, TransactionType.Insert );

         Assert.IsTrue( result );
      }

      [TestMethod]
      public void ModelIsNotValidOnInsert_IfUserNameIsNotUnique()
      {
         var user = CreateNewUser();
         user.UserName = Users.ModelData[0].UserName;

         var result = _validator.ModelIsValid( user, TransactionType.Insert );

         Assert.IsFalse( result );
      }

      [TestMethod]
      public void ModelIsValidOnUpdate_IfUserNameIsUnique()
      {
         var user = CopyUser( Users.ModelData[0] );
         user.UserName = user.UserName + "somthing";

         var result = _validator.ModelIsValid( user, TransactionType.Update );

         Assert.IsTrue( result );
      }

      [TestMethod]
      public void ModelIsNotValidOnUpdate_IfUsernameIsNotUnique()
      {
         var user = CopyUser( Users.ModelData[0] );
         user.UserName = Users.ModelData[1].UserName;

         var result = _validator.ModelIsValid( user, TransactionType.Update );

         Assert.IsFalse( result );
      }

      [TestMethod]
      public void ModelIsValidOnUpdate_IfUsernameIsUnchangedForUser()
      {
         var user = CopyUser( Users.ModelData[0] );
         user.FirstName = user.FirstName + "somthing";

         var result = _validator.ModelIsValid( user, TransactionType.Update );

         Assert.IsTrue( result );
      }

      [TestMethod]
      public void NoMessages_IfModelIsValid()
      {
         var user = CreateNewUser();

         _validator.ModelIsValid( user, TransactionType.Insert );

         Assert.AreEqual( 0, _validator.Messages.Count );
      }

      [TestMethod]
      public void Message_IfModelIsNotValid()
      {
         var user = CreateNewUser();
         user.UserName = Users.ModelData[0].UserName;

         _validator.ModelIsValid( user, TransactionType.Insert );

         Assert.AreEqual( 1, _validator.Messages.Count );
         var message = _validator.Messages.First();
         Assert.AreEqual( "UserName", message.Key );
         Assert.AreEqual( String.Format( ErrorMessages.UsernameIsNotUnique, user.UserName ), message.Value );
      }
   }
}
