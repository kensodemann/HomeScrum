using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using HomeScrum.Data.Validators;
using HomeScrum.Data.Domain;
using HomeScrum.Data.Repositories;
using HomeScrum.Common.TestData;

namespace HomeScrum.Data.UnitTest.Validators
{
   [TestClass]
   public class ProjectStatusValidatorTest
   {
      private Mock<IRepository<ProjectStatus, Guid>> _repository;
      private ProjectStatusValidator _validator;

      [TestInitialize]
      public void InitializeTest()
      {
         _repository = new Mock<IRepository<ProjectStatus, Guid>>();
         _repository.Setup( x => x.GetAll() ).Returns( ProjectStatuses.ModelData );
         _validator = new ProjectStatusValidator( _repository.Object );
      }


      [TestMethod]
      public void MessagesContainsUniqueNameMessage_IfNameNotUnique()
      {
         var model = new ProjectStatus();
         model.Name = ProjectStatuses.ModelData[1].Name;
         model.Id = ProjectStatuses.ModelData[0].Id;

         var result = _validator.ModelIsValid( model, TransactionType.All );

         Assert.IsFalse( result );
         Assert.AreEqual( 1, _validator.Messages.Count );
         foreach (var message in _validator.Messages)
         {
            Assert.AreEqual( "Name", message.Key );
            Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Project Status", model.Name ), message.Value );
         }
      }
   }
}
