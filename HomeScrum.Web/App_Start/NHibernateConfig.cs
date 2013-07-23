using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HomeScrum.Common.Utility;
using NHibernate;
using NHibernate.Context;

namespace HomeScrum.Web
{
   public static class NHibernateConfig
   {
      public static void Configure()
      {
         var iocKernel = NinjectWebCommon.Kernel;

         // Experimental code - don't actually need NHibernate to do any injection right now, but
         // uncommenting this line and setting up a proxy factory in Ninject setup would do it.
         //
         //NHibernate.Cfg.Environment.BytecodeProvider = new NinjectBytecodeProvider( iocKernel );

         var configuration = new Configuration();
         configuration.SetProperty( NHibernate.Cfg.Environment.CurrentSessionContextClass, typeof( WebSessionContext ).FullName );
         configuration.Configure();
         var sessionFactory = configuration.BuildSessionFactory();

         iocKernel.Bind<ISessionFactory>().ToConstant( sessionFactory );
      }
   }
}