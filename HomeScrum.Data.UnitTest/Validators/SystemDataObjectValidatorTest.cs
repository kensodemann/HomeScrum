using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Validators;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Common.TestData;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class SystemDataObjectValidatorTest
   {
      private Mock<IDataObjectRepository<WorkItemType>> _repository;
      private SystemDataObjectValidator<WorkItemType> _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<WorkItemType>>();
         _repository.Setup( x => x.GetAll() ).Returns( WorkItemTypes.ModelData );
         _validator = new SystemDataObjectValidator<WorkItemType>( _repository.Object );
      }


      [TestMethod]
      public void ModelIsValid_GetsAllItemFromRepository()
      {
         var model = new WorkItemType();
         var result = _validator.ModelIsValid( model );

         _repository.Verify( x => x.GetAll(), Times.Once() );
      }


      [TestMethod]
      public void ModelIsValidReturnsTrue_IfValueNameEmpty()
      {
         var model = new WorkItemType();
         var result = _validator.ModelIsValid( model );

         Assert.IsTrue( result );
      }

      [TestMethod]
      public void ModelIsValidReturnsFalse_IfNameOfNewObjectAlreadyExists()
      {
         var model = new WorkItemType();
         model.Name = WorkItemTypes.ModelData[1].Name;

         var result = _validator.ModelIsValid( model );

         Assert.IsFalse( result );
      }

      [TestMethod]
      public void ModelIsValidReturnsFalse_IfNameExistsOnValueWithDifferentId()
      {
         var model = new WorkItemType();
         model.Name = WorkItemTypes.ModelData[1].Name;
         model.Id = WorkItemTypes.ModelData[0].Id;

         var result = _validator.ModelIsValid( model );

         Assert.IsFalse( result );
      }

      [TestMethod]
      public void ModelIsValidReturnsTrue_IfNameExistsOnValueWithMatchingId()
      {
         var model = new WorkItemType();
         model.Name = WorkItemTypes.ModelData[1].Name;
         model.Id = WorkItemTypes.ModelData[1].Id;

         var result = _validator.ModelIsValid( model );

         Assert.IsTrue( result );
      }

      //[TestMethod]
      //public void ValidatorUsesResourceString_IfValidResourceGiven()
      //{
      //   var model = new WorkItemType();
      //   model.Name = WorkItemTypes.ModelData[1].Name;
      //   _validator.ErrorMessage = "This is my error message";
      //   _validator.ErrorMessageResourceName = "TestErrorMessage";
      //   _validator.ErrorMessageResourceType = typeof( TestStrings );

      //   var result = _validator.GetValidationResult( model, new ValidationContext( model, null, null ) );

      //   Assert.AreEqual( TestStrings.TestErrorMessage, result.ErrorMessage );
      //}

      // TODO: Add test for:
      //    o rename the validator class
      //    o incomplete resource (give no ErrorMessageResourceName)
      //    o invalid resource (give bad ErrorMessageResourceName)
      //    o no error message given (use default generic message)
   }
}
