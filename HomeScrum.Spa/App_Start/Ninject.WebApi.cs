﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;
using HomeScrum.Data.Services;
using HomeScrum.Spa.Controllers;
using Ninject;
using Ninject.Modules;

namespace HomeScrum.Spa.App_Start
{
   /// <summary>
   /// Resolves Dependencies Using Ninject
   /// </summary>
   public class NinjectHttpResolver : IDependencyResolver, IDependencyScope
   {
      public IKernel Kernel { get; private set; }
      public NinjectHttpResolver( params NinjectModule[] modules )
      {
         Kernel = new StandardKernel( modules );
      }

      public object GetService( Type serviceType )
      {
         return Kernel.TryGet( serviceType );
      }

      public IEnumerable<object> GetServices( Type serviceType )
      {
         return Kernel.GetAll( serviceType );
      }

      public void Dispose()
      {
         //Do Nothing
      }

      public IDependencyScope BeginScope()
      {
         return this;
      }
   }


   // List and Describe Necessary HttpModules
   // This class is optional if you already Have NinjectMvc
   public class NinjectHttpModules
   {
      //Return Lists of Modules in the Application
      public static NinjectModule[] Modules
      {
         get
         {
            return new[] { new MainModule() };
         }
      }

      //Main Module For Application
      public class MainModule : NinjectModule
      {
         public override void Load()
         {
            //TODO: Bind to Concrete Types Here
         }
      }
   }


   /// <summary>
   /// Its job is to Register Ninject Modules and Resolve Dependencies
   /// </summary>
   public class NinjectHttpContainer
   {
      private static NinjectHttpResolver _resolver;

      public static IKernel Kernel { get { return _resolver.Kernel; } }

      //Register Ninject Modules
      public static void RegisterModules( NinjectModule[] modules )
      {
         _resolver = new NinjectHttpResolver( modules );
         GlobalConfiguration.Configuration.DependencyResolver = _resolver;
      }

      //Manually Resolve Dependencies
      public static T Resolve<T>()
      {
         return _resolver.Kernel.Get<T>();
      }
   }

}