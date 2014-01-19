using HomeScrum.Data.Repositories;
using HomeScrum.Data.Services;
using HomeScrum.Spa.Providers;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HomeScrum.Spa.App_Start
{
   //  A small Library to configure Ninject (A Dependency Injection Library) with a WebAPI Application. 
   //  To configure, take the following steps.
   // 
   //  1. Install Packages Ninject and Ninject.Web.Common 
   //  2. Remove NinjectWebCommon.cs in your App_Start Directory
   //  3. Add this file to your project  (preferrably in the App_Start Directory)  
   //  4. Add Your Bindings to Concrete Types to Load method of the main Module. You can add as many additional modules as you want, simply add them to the Modules property of the NinjectModules class
   //  5. Add the following Line to your Global.asax
   //          NinjectContainer.RegisterModules(NinjectHttpModules.Modules);  
   //  You are done. 


   /// <summary>
   /// Resolves Dependencies Using Ninject
   /// </summary>
   public class NinjectResolver : IDependencyResolver
   {
      public IKernel Kernel { get; private set; }
      public NinjectResolver( params NinjectModule[] modules )
      {
         Kernel = new StandardKernel( modules );
      }

      public object GetService( Type serviceType )
      {
         var service = Kernel.TryGet( serviceType );
         return service;
      }

      public IEnumerable<object> GetServices( Type serviceType )
      {
         return Kernel.GetAll( serviceType );
      }
   }


   // List and Describe Necessary Modules
   public class NinjectModules
   {
      //Return Lists of Modules in the Application
      public static NinjectModule[] Modules
      {
         get
         {
            //Return Modules you want to use for DI
            return new[] { new MainModule() };
         }
      }

      //Main Module For Application. 
      public class MainModule : NinjectModule
      {
         public override void Load()
         {
            Kernel.Bind<ISecurityService>().To( typeof( SecurityService ) ).InSingletonScope();
            Kernel.Bind<IWebSecurity>().ToConstant( new WebSecurityWrapper() );
         }
      }

      //You can create as many Modules as you wish
   }


   /// <summary>
   /// Its job is to Register Ninject Modules and Resolve Dependencies
   /// </summary>
   public class NinjectContainer
   {
      private static NinjectResolver _resolver;

      public static IKernel Kernel { get { return _resolver.Kernel; } }

      //Register Ninject Modules
      public static void RegisterModules( NinjectModule[] modules )
      {
         _resolver = new NinjectResolver( modules );
         DependencyResolver.SetResolver( _resolver );
      }

      //Manually Resolve Dependencies
      public static T Resolve<T>()
      {
         return _resolver.Kernel.Get<T>();
      }
   }
}