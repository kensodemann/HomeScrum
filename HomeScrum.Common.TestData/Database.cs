using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Data.SqlClient;

namespace HomeScrum.Common.TestData
{
   public class Database
   {
      private static ISessionFactory _sessionFactory;
      private static Configuration _configuration;

      public static void Initialize()
      {
         LoadConfiguration();
         CreateSessionFactory();
      }

      private static void CreateSessionFactory()
      {
         try
         {
            _sessionFactory = _configuration.BuildSessionFactory();
         }
         catch (SqlException e)
         {
            throw new Exception( "You probably need to create HomeScrumUnitTest in the default local SQL-Server instance.", e );
         }
      }

      private static void LoadConfiguration()
      {
         _configuration = new Configuration();
         _configuration.Configure();
         AddAssemblies();
      }

      private static void AddAssemblies()
      {
         _configuration.AddAssembly( typeof( WorkItemType ).Assembly );
      }


      public static void Build()
      {
         new SchemaExport( _configuration ).Execute( false, true, false );
      }

      public static ISession GetSession()
      {
         return _sessionFactory.OpenSession();
      }
   }
}
