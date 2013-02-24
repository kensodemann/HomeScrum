using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScrum.Data.UnitTest
{
   public class TestData
   {
      private static ISessionFactory _sessionFactory;
      private static Configuration _configuration;

      public static void Initialize()
      {
         _configuration = new Configuration();
         _configuration.Configure();
         _configuration.AddAssembly( typeof( WorkItemType ).Assembly );
         _sessionFactory = _configuration.BuildSessionFactory();
      }


      public static void BuildDatabase()
      {
         new SchemaExport( _configuration ).Execute( false, true, false );
         CreateInitialWorkItemTypes();
      }

      #region WorkItemTypes
      private static void CreateInitialWorkItemTypes()
      {

         using (ISession session = _sessionFactory.OpenSession())
         using (ITransaction transaction = session.BeginTransaction())
         {
            foreach (var workItemType in WorkItemTypes)
               session.Save( workItemType );
            transaction.Commit();
         }
      }

      public static readonly WorkItemType[] WorkItemTypes = new[]
      {
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 1",
            Description="This is type number 1",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 2",
            Description="This is type number 2",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 3",
            Description="Active, Not a Task, Predefined",
            StatusCd='A',
            IsTask='N',
            IsPredefined='Y'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 4",
            Description="Active, Task, Not Predefined",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='N'
         },
         new WorkItemType ()
         {
            Id=Guid.NewGuid(),
            Name="Type 5",
            Description="Not Active, Task, Predefined",
            StatusCd='A',
            IsTask='Y',
            IsPredefined='Y'
         }
      };
      #endregion
   }
}
