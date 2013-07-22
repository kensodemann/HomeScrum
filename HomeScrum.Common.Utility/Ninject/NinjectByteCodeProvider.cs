using System;
using Ninject;
using NHibernate.Bytecode;
using NHibernate.Properties;
using NHibernate.Type;

//
// NOTE: May want to move this to its own assembly.
namespace HomeScrum.Common.Utility.Ninject
{
   // Obtained from here: https://code.google.com/p/unhaddins/source/browse/#hg%2FuNhAddIns%2FuNhAddIns.NinjectAdapters%2FBytecodeProvider
   public class NinjectBytecodeProvider : IBytecodeProvider, IInjectableCollectionTypeFactoryClass
   {

      public NinjectBytecodeProvider( IKernel Kernel )
      {
         kernel = Kernel;
         objectsFactory = new ObjectsFactory( Kernel );
         collectionTypeFactory = new DefaultCollectionTypeFactory();
      }

      protected readonly IObjectsFactory objectsFactory;
      protected readonly IKernel kernel;
      protected ICollectionTypeFactory collectionTypeFactory;

      #region IBytecodeProvider Members
      ICollectionTypeFactory IBytecodeProvider.CollectionTypeFactory
      {
         get { return collectionTypeFactory; }
      }

      IReflectionOptimizer IBytecodeProvider.GetReflectionOptimizer( System.Type clazz, IGetter[] getters, ISetter[] setters )
      {
         return new ReflectionOptimizer( kernel, clazz, getters, setters );
      }

      IObjectsFactory IBytecodeProvider.ObjectsFactory
      {
         get { return objectsFactory; }
      }

      IProxyFactoryFactory IBytecodeProvider.ProxyFactoryFactory
      {
         get { return new ProxyFactoryFactory( kernel ); }
      }
      #endregion

      #region IInjectableCollectionTypeFactoryClass Members
      void IInjectableCollectionTypeFactoryClass.SetCollectionTypeFactoryClass( System.Type type )
      {
         collectionTypeFactory = (ICollectionTypeFactory)Activator.CreateInstance( type );
      }

      void IInjectableCollectionTypeFactoryClass.SetCollectionTypeFactoryClass( string typeAssemblyQualifiedName )
      {
         ((IInjectableCollectionTypeFactoryClass)this).SetCollectionTypeFactoryClass( System.Type.GetType( typeAssemblyQualifiedName, true ) );
      }
      #endregion
   }
}
