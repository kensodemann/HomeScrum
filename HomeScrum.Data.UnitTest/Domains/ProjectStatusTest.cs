﻿using System;
using HomeScrum.Common.TestData;
using HomeScrum.Data.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HomeScrum.Data.UnitTest.Domains
{
   [TestClass]
   public class ProjectStatusTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext ctx )
      {
         Database.Initialize();
      }

      [TestInitialize]
      public void InitializeTest()
      {
         Database.Build();
         ProjectStatuses.Load();
      }


      [TestMethod]
      public void IsNotValid_IfDifferentItemWithSameNameExists()
      {
         var item = new ProjectStatus()
         {
            Id = Guid.NewGuid(),
            Name = ProjectStatuses.ModelData[0].Name,
            Description = "This is just for testing",
            IsActive = true,
            SortSequence = 0,
            StatusCd = 'A',
            IsPredefined = false
         };

         Assert.IsFalse( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 1, messages.Count );
         Assert.AreEqual( String.Format( ErrorMessages.NameIsNotUnique, "Project Status", item.Name ), messages["Name"] );
      }

      [TestMethod]
      public void IsValid_IfExistingValidItem()
      {
         var item = ProjectStatuses.ModelData[0];

         Assert.IsTrue( item.IsValidFor( TransactionType.All ) );
         var messages = item.GetErrorMessages();
         Assert.AreEqual( 0, messages.Count );
      }
   }
}