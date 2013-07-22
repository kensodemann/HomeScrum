﻿using System;
using NHibernate.Bytecode;
using Ninject;

namespace HomeScrum.Common.Utility.Ninject
{
   // Obtained from here: https://code.google.com/p/unhaddins/source/browse/#hg%2FuNhAddIns%2FuNhAddIns.NinjectAdapters%2FBytecodeProvider
   public class ObjectsFactory : IObjectsFactory
   {
      public ObjectsFactory( IKernel Kernel )
      {
         kernel = Kernel;
      }

      private readonly IKernel kernel;


      #region IObjectsFactory Members
      object IObjectsFactory.CreateInstance( System.Type type, params object[] ctorArgs )
      {
         return Activator.CreateInstance( type, ctorArgs );
      }

      object IObjectsFactory.CreateInstance( System.Type type, bool nonPublic )
      {
         return Activator.CreateInstance( type, nonPublic );
      }

      object IObjectsFactory.CreateInstance( System.Type type )
      {
         return Activator.CreateInstance( type );
      }
      #endregion
   }
}
