using System;
using Ninject;
using NHibernate.Bytecode;

namespace HomeScrum.Common.Utility.Ninject
{
   // Obtained from here: https://code.google.com/p/unhaddins/source/browse/#hg%2FuNhAddIns%2FuNhAddIns.NinjectAdapters%2FBytecodeProvider
   public class ProxyFactoryFactory : IProxyFactoryFactory
   {
      public ProxyFactoryFactory( IKernel Kernel )
      {
         kernel = Kernel;
      }

      private readonly IKernel kernel;


      #region IProxyFactoryFactory Members
      NHibernate.Proxy.IProxyFactory IProxyFactoryFactory.BuildProxyFactory()
      {
         return kernel.Get<NHibernate.Proxy.IProxyFactory>();
      }

      public bool IsInstrumented( Type entityClass )
      {
         return false;
      }

      NHibernate.Proxy.IProxyValidator IProxyFactoryFactory.ProxyValidator
      {
         get { return new Proxy.ProxyTypeValidator(); }
      }

      public bool IsProxy( object entity )
      {
         return entity is NHibernate.Proxy.INHibernateProxy;
      }
      #endregion
   }
}
