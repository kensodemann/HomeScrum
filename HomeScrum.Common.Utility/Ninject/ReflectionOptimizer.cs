using System;
using Ninject;
using NHibernate.Properties;

namespace HomeScrum.Common.Utility.Ninject
{
   // Obtained from here: https://code.google.com/p/unhaddins/source/browse/#hg%2FuNhAddIns%2FuNhAddIns.NinjectAdapters%2FBytecodeProvider
   public class ReflectionOptimizer : NHibernate.Bytecode.Lightweight.ReflectionOptimizer
   {
      public ReflectionOptimizer( IKernel Kernel, Type mappedType, IGetter[] getters, ISetter[] setters )
         : base( mappedType, getters, setters )
      {
         kernel = Kernel;
      }

      protected IKernel kernel;

      /// <summary>
      /// Ignore this check
      /// </summary>
      /// <param name="type"></param>
      protected override void ThrowExceptionForNoDefaultCtor( Type type ) { }

      public override object CreateInstance()
      {
         return kernel.Get( mappedType );
      }
   }
}
