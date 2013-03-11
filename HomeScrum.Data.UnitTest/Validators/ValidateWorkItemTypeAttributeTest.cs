using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Validators;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using System.ComponentModel.DataAnnotations;
using HomeScrum.Common.TestData;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class ValidateWorkItemTypeAttributeTest
   {
      private Mock<IDataObjectRepository<WorkItemType>> _repository;
      private ValidateWorkItemTypeAttribute _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<WorkItemType>>();
         _repository.Setup( x => x.GetAll() ).Returns( WorkItemTypes.ModelData );
         _validator = new ValidateWorkItemTypeAttribute()
         {
            Repository = _repository.Object
         };
      }


      [TestMethod]
      public void Validator_GetsAllWorkItemTypes()
      {
         var workItemType = new WorkItemType();
         var result = _validator.GetValidationResult( workItemType, new ValidationContext( workItemType, null, null ) );

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void ValidatorReturnsErrorMessage_IfNameExistsOnNewObject()
      {
         var workItemType = new WorkItemType();
         workItemType.Name = WorkItemTypes.ModelData[1].Name;
         _validator.ErrorMessage = "This is my error message";

         var result = _validator.GetValidationResult( workItemType, new ValidationContext( workItemType, null, null ) );

         Assert.AreNotEqual( ValidationResult.Success, result );
         Assert.AreEqual( "This is my error message", result.ErrorMessage );
      }
   }
}
