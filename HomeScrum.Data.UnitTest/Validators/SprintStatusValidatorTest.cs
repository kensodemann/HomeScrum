﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Validators;
using HomeScrum.Common.TestData;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class SprintStatusValidatorTest
   {
      private Mock<IDataObjectRepository<SprintStatus>> _repository;
      private SprintStatusValidator _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IDataObjectRepository<SprintStatus>>();
         _repository.Setup( x => x.GetAll() ).Returns( SprintStatuses.ModelData );
         _validator = new SprintStatusValidator( _repository.Object );
      }


      [TestMethod]
      public void MessagesContainsUniqueNameMessage_IfNameNotUnique()
      {
         var model = new SprintStatus();
         model.Name = SprintStatuses.ModelData[1].Name;
         model.Id = SprintStatuses.ModelData[0].Id;

         var result = _validator.ModelIsValid( model );

         Assert.IsFalse( result );
         Assert.AreEqual( 1, _validator.Messages.Count );
         foreach (var message in _validator.Messages)
         {
            Assert.AreEqual( "Name", message.Key );
            Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Sprint Status", model.Name ), message.Value );
         }
      }
   }
}