using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Common.TestData;
using Moq;
using HomeScrum.Data.Validators;
using HomeScrum.Data.Repositories;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class AcceptanceCriteriaStatusValidatorTest
   {
      private Mock<IRepository<AcceptanceCriteriaStatus>> _repository;
      private AcceptanceCriteriaStatusValidator _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IRepository<AcceptanceCriteriaStatus>>();
         _repository.Setup( x => x.GetAll() ).Returns( AcceptanceCriteriaStatuses.ModelData );
         _validator = new AcceptanceCriteriaStatusValidator( _repository.Object );
      }


      [TestMethod]
      public void MessagesContainsUniqueNameMessage_IfNameNotUnique()
      {
         var model = new AcceptanceCriteriaStatus();
         model.Name = AcceptanceCriteriaStatuses.ModelData[1].Name;
         model.Id = AcceptanceCriteriaStatuses.ModelData[0].Id;

         var result = _validator.ModelIsValid( model );

         Assert.IsFalse( result );
         Assert.AreEqual( 1, _validator.Messages.Count );
         foreach (var message in _validator.Messages)
         {
            Assert.AreEqual( "Name", message.Key );
            Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Acceptance Criteria Status", model.Name ), message.Value );
         }
      }
   }
}
