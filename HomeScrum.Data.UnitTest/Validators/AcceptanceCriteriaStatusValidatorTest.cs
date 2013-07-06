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
      private Mock<IRepository<AcceptanceCriterionStatus>> _repository;
      private AcceptanceCriteriaStatusValidator _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IRepository<AcceptanceCriterionStatus>>();
         _repository.Setup( x => x.GetAll() ).Returns( AcceptanceCriteriaStatuses.ModelData );
         _validator = new AcceptanceCriteriaStatusValidator( _repository.Object );
      }


      [TestMethod]
      public void MessagesContainsUniqueNameMessage_IfNameNotUnique_OnInsert()
      {
         var model = new AcceptanceCriterionStatus();
         model.Name = AcceptanceCriteriaStatuses.ModelData[1].Name;
         model.Id = AcceptanceCriteriaStatuses.ModelData[0].Id;

         var result = _validator.ModelIsValid( model, TransactionType.Insert );

         Assert.IsFalse( result );
         Assert.AreEqual( 1, _validator.Messages.Count );
         foreach (var message in _validator.Messages)
         {
            Assert.AreEqual( "Name", message.Key );
            Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Acceptance Criteria Status", model.Name ), message.Value );
         }
      }

      [TestMethod]
      public void MessagesContainsUniqueNameMessage_IfNameNotUnique_OnUpdate()
      {
         var model = new AcceptanceCriterionStatus();
         model.Name = AcceptanceCriteriaStatuses.ModelData[1].Name;
         model.Id = AcceptanceCriteriaStatuses.ModelData[0].Id;

         var result = _validator.ModelIsValid( model, TransactionType.Update );

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
