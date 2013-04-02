using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Cfg;

namespace HomeScrum.Data.SqlServer.Helpers
{
   public class NHibernateHelper
   {
      private static ISessionFactory _sessionFactory;

      private static ISessionFactory SessionFactory
      {
         get
         {
            if (_sessionFactory == null)
            {
               var configuration = new Configuration();
               configuration.Configure();
               configuration.AddAssembly( typeof( WorkItemType ).Assembly );
               _sessionFactory = configuration.BuildSessionFactory();
            }
            return _sessionFactory;
         }
      }

      public static ISession OpenSession()
      {
         return SessionFactory.OpenSession();
      }
   }
}
