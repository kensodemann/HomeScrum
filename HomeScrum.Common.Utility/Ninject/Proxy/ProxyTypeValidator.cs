using NHibernate.Proxy;
using System;

namespace HomeScrum.Common.Utility.Ninject.Proxy
{
   // Obtained from here: https://code.google.com/p/unhaddins/source/browse/#hg%2FuNhAddIns%2FuNhAddIns.NinjectAdapters%2FBytecodeProvider
   class ProxyTypeValidator : DynProxyTypeValidator
   {
      /// <summary>
      /// Ignore this check
      /// </summary>
      /// <param name="type"></param>
      /// <remarks>Do nothing. Since we're using constructor
      /// injection with NHibernate, our entities will have parameters.</remarks>
      protected override void CheckHasVisibleDefaultConstructor( Type type ) { }
   }
}
