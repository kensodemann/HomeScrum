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
      public void ValidatorGetsAllWorkItemTypes_IfValueWorkItemType()
      {
         var model = new WorkItemType();
         var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }

      [TestMethod]
      public void ValidatorDoesNotGetAll_IfValueNotWorkItemType()
      {
         var model = new WorkItemStatus();
         model.Name = WorkItemTypes.ModelData[1].Name;
         _validator.ErrorMessage = "This is my error message";

         var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

         _repository.Verify( x => x.GetAll(), Times.Never() );
      }

      [TestMethod]
      public void ValidatorReturnsSuccess_IfValueNotWorkItemType()
      {
         var model = new WorkItemStatus();
         model.Name = WorkItemTypes.ModelData[1].Name;
         _validator.ErrorMessage = "This is my error message";

         var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

         Assert.AreEqual( ValidationResult.Success, result );
      }

      [TestMethod]
      public void ValidatorReturnsSuccess_IfValueNameEmpty()
      {
         var model = new WorkItemStatus();
         _validator.ErrorMessage = "This is my error message";

         var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

         Assert.AreEqual( ValidationResult.Success, result );
      }

      [TestMethod]
      public void ValidatorReturnsErrorMessage_IfNameExistsOnNewObject()
      {
         var model = new WorkItemType();
         model.Name = WorkItemTypes.ModelData[1].Name;
         _validator.ErrorMessage = "This is my error message";

         var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

         Assert.AreNotEqual( ValidationResult.Success, result );
         Assert.AreEqual( "This is my error message", result.ErrorMessage );
      }

      [TestMethod]
      public void ValidatorReturnsErrorMessage_IfNameExistsOnValueWithDifferentId()
      {
         var model = new WorkItemType();
         model.Name = WorkItemTypes.ModelData[1].Name;
         model.Id = WorkItemTypes.ModelData[0].Id;
         _validator.ErrorMessage = "This is my error message";

         var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

         Assert.AreNotEqual( ValidationResult.Success, result );
         Assert.AreEqual( "This is my error message", result.ErrorMessage );
      }

      [TestMethod]
      public void ValidatorReturnsSuccess_IfNameExistsOnValueWithMatchingId()
      {
         var model = new WorkItemType();
         model.Name = WorkItemTypes.ModelData[1].Name;
         model.Id = WorkItemTypes.ModelData[1].Id;
         _validator.ErrorMessage = "This is my error message";

         var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

         Assert.AreEqual( ValidationResult.Success, result );
      }
   }
}
