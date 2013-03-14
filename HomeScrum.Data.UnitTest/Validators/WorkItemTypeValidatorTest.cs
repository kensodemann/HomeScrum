using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Validators;
using HomeScrum.Common.TestData;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class WorkItemTypeValidatorTest
   {
      private Mock<IDataObjectRepository<WorkItemType>> _repository;
      private WorkItemTypeValidator _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<WorkItemType>>();
         _repository.Setup( x => x.GetAll() ).Returns( WorkItemTypes.ModelData );
         _validator = new WorkItemTypeValidator( _repository.Object );
      }


      [TestMethod]
      public void MessagesContainsUniqueNameMessage_IfNameNotUnique()
      {
         var model = new WorkItemType();
         model.Name = WorkItemTypes.ModelData[1].Name;
         model.Id = WorkItemTypes.ModelData[0].Id;

         var result = _validator.ModelIsValid( model );

         Assert.IsFalse( result );
         Assert.AreEqual( 1, _validator.Messages.Count );
         foreach (var message in _validator.Messages)
         {
            Assert.AreEqual( "Name", message.Key );
            Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Work Item Type", model.Name ), message.Value );
         }
      }
   }
}
