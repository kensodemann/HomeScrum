﻿using NHibernate;
using NHibernate.Cfg;

namespace HomeScrum.Common.Utility
{
   public class NHibernateHelper
   {
      private static ISessionFactory _sessionFactory;

      public static ISessionFactory SessionFactory
      {
         get
         {
            if (_sessionFactory == null)
            {
               var configuration = new Configuration();
               configuration.Configure();
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