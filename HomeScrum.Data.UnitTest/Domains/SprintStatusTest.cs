﻿using System;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Moq;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class SprintStatusTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         SetupSession();

         Database.Build( _session );
         SprintStatuses.Load( _sessionFactory.Object );
      }

      private void SetupSession()
      {
         _session = Database.SessionFactory.OpenSession();
         _sessionFactory = new Mock<ISessionFactory>();
         _sessionFactory.Setup( x => x.GetCurrentSession() ).Returns( _session );
      }

      [TestCleanup]
      public void CleanupTest()
      {
         _session.Dispose();
      }

      private ISession _session;
      private Mock<ISessionFactory> _sessionFactory;


      [TestMethod]
      public void IsNotValid_IfDifferentItemWithSameNameExists()
      {
         var item = new SprintStatus( _sessionFactory.Object )
         {
            Id = Guid.NewGuid(),
            Name = SprintStatuses.ModelData[0].Name,
            Description = "This is just for testing",
            Category = SprintStatusCategory.Active,
            SortSequence = 0,
            StatusCd = 'A',
            IsPredefined = false
         };

         Assert.IsFalse( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Sprint Status", item.Name ), messages["Name"] );
      }

      [TestMethod]
      public void IsValid_IfExistingValidItem()
      {
         var item = SprintStatuses.ModelData[0];

         Assert.IsTrue( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 0, messages.Count );
      }
   }
}
