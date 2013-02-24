using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HomeScrum.Data.Repositories;
using HomeScrum.Data.Domain;
using HomeScrum.Data.SqlServer;
using System.Collections.Generic;
using System.Linq;

namespace HomeScrum.Data.UnitTest
{
   [TestClass]
   public class WorkItemTypeRepositoryTest
   {
      [ClassInitialize]
      public static void InitializeClass( TestContext context )
      {
         TestData.Initialize();
      }


      [TestInitialize]
      public void InitializeTest()
      {
         TestData.BuildDatabase();
      }


      [TestMethod]
      public void GetAll_ReturnsAllWorkItemTypes()
      {
         IDataObjectRepository<WorkItemType> repository = new DataObjectRepository<WorkItemType>();

         var workItemTypes = repository.GetAll();

         Assert.AreEqual( TestData.WorkItemTypes.GetLength( 0 ), workItemTypes.Count );
         foreach (var wit in TestData.WorkItemTypes)
         {
            AssertCollectionContainsWorkItemType( workItemTypes, wit );
         }
      }


      private void AssertCollectionContainsWorkItemType( ICollection<WorkItemType> workItemTypes, WorkItemType workItemType )
      {
         var workItemTypeFromCollection = workItemTypes.FirstOrDefault( x => x.Id == workItemType.Id );

         Assert.IsNotNull( workItemTypeFromCollection );
         AssertWorkItemTypesAreEqual( workItemType, workItemTypeFromCollection );
      }

      private static void AssertWorkItemTypesAreEqual( WorkItemType expected, WorkItemType actual )
      {
         Assert.AreNotSame( expected, actual );
         Assert.AreEqual( expected.Id, actual.Id );
         Assert.AreEqual( expected.Name, actual.Name );
         Assert.AreEqual( expected.Description, actual.Description );
         Assert.AreEqual( expected.StatusCd, actual.StatusCd );
         Assert.AreEqual( expected.IsTask, actual.IsTask );
         Assert.AreEqual( expected.IsPredefined, actual.IsPredefined );
      }
   }
}
