using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using Moq;
using HomeScrum.Common.TestData;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class WorkItemStatusValidatorTest
   {
      private Mock<IRepository<WorkItemStatus, Guid>> _repository;
      private WorkItemStatusValidator _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IRepository<WorkItemStatus, Guid>>();
         _repository.Setup( x => x.GetAll() ).Returns( WorkItemStatuses.ModelData );
         _validator = new WorkItemStatusValidator( _repository.Object );
      }


      [TestMethod]
      public void MessagesContainsUniqueNameMessage_IfNameNotUnique()
      {
         var model = new WorkItemStatus();
         model.Name = WorkItemStatuses.ModelData[1].Name;
         model.Id = WorkItemStatuses.ModelData[0].Id;

         var result = _validator.ModelIsValid( model, TransactionType.All );

         Assert.IsFalse( result );
         Assert.AreEqual( 1, _validator.Messages.Count );
         foreach (var message in _validator.Messages)
         {
            Assert.AreEqual( "Name", message.Key );
            Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Work Item Status", model.Name ), message.Value );
         }
      }
   }
}
