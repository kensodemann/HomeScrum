using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;

namespace HomeScrum.Data.UnitTest.TestData
{
   public class Database
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
