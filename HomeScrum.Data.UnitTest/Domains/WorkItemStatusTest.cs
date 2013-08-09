using System;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Moq;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class WorkItemStatusTest
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
         WorkItemStatuses.Load( _sessionFactory.Object );
      }

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;


      [TestMethod]
      public void IsNotValid_IfDifferentItemWithSameNameExists()
      {
         var item = new WorkItemStatus( _sessionFactory.Object )
         {
            Id = Guid.NewGuid(),
            Name = WorkItemStatuses.ModelData[0].Name,
            Description = "This is just for testing",
            IsOpenStatus = true,
            SortSequence = 0,
            StatusCd = 'A',
            IsPredefined = false
         };

         Assert.IsFalse( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Work Item Status", item.Name ), messages["Name"] );
      }

      [TestMethod]
      public void IsValid_IfExistingValidItem()
      {
         var item = new WorkItemStatus( _sessionFactory.Object )
         {
            Id = WorkItemStatuses.ModelData[0].Id,
            Name = WorkItemStatuses.ModelData[0].Name,
            Description = WorkItemStatuses.ModelData[0].Description,
            IsOpenStatus = WorkItemStatuses.ModelData[0].IsOpenStatus,
            SortSequence = WorkItemStatuses.ModelData[0].SortSequence,
            StatusCd = WorkItemStatuses.ModelData[0].StatusCd,
            IsPredefined = WorkItemStatuses.ModelData[0].IsPredefined
         };

         Assert.IsTrue( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 0, messages.Count );
      }
   }
}
