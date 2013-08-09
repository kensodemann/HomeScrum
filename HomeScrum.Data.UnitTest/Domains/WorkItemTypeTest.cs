using System;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Moq;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class WorkItemTypeTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         _session = Database.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );

         Database.Build( _session );
         WorkItemTypes.Load( _sessionFactory.Object );
      }

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;


      [TestMethod]
      public void IsNotValid_IfDifferentItemWithSameNameExists()
      {
         var item = new WorkItemType( _sessionFactory.Object )
         {
            Id = Guid.NewGuid(),
            Name = WorkItemTypes.ModelData[0].Name,
            Description = "This is just for testing",
            IsTask = true,
            SortSequence = 0,
            StatusCd = 'A',
            IsPredefined = false
         };

         Assert.IsFalse( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Work Item Type", item.Name ), messages["Name"] );
      }

      [TestMethod]
      public void IsValid_IfExistingValidItem()
      {
         var item = WorkItemTypes.ModelData[0];

         Assert.IsTrue( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 0, messages.Count );
      }
   }
}
