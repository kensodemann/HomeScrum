[assembly: WebActivator.PreApplicationStartMethod( typeof( HomeScrum.Web.App_Start.NinjectWebCommon ), "Start" )]
[assembly: WebActivator.ApplicationShutdownMethodAttribute( typeof( HomeScrum.Web.App_Start.NinjectWebCommon ), "Stop" )]

namespace HomeScrum.Web.App_Start
{
   using Microsoft.Web.Infrastructure.DynamicModuleHelper;
   using Ninject;
   using Ninject.Web.Common;
   using System;
   using System.Web;
   using HomeScrum.Data.Domain;
   using HomeScrum.Data.Repositories;
   using HomeScrum.Data.SqlServer;

   public static class NinjectWebCommon
   {
      private static readonly Bootstrapper bootstrapper = new Bootstrapper();

      /// <summary>
      /// Starts the application
      /// </summary>
      public static void Start()
      {
         DynamicModuleUtility.RegisterModule( typeof( OnePerRequestHttpModule ) );
         DynamicModuleUtility.RegisterModule( typeof( NinjectHttpModule ) );
         bootstrapper.Initialize( CreateKernel );
      }

      /// <summary>
      /// Stops the application.
      /// </summary>
      public static void Stop()
      {
         bootstrapper.ShutDown();
      }

      /// <summary>
      /// Creates the kernel that will manage your application.
      /// </summary>
      /// <returns>The created kernel.</returns>
      private static IKernel CreateKernel()
      {
         var kernel = new StandardKernel();
         kernel.Bind<Func<IKernel>>().ToMethod( ctx => () => new Bootstrapper().Kernel );
         kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

         RegisterServices( kernel );
         return kernel;
      }

      /// <summary>
      /// Load your modules or register your services here!
      /// </summary>
      /// <param name="kernel">The kernel.</param>
      private static void RegisterServices( IKernel kernel )
      {
         RegisterRepositories( kernel );
      }

      private static void RegisterRepositories( IKernel kernel )
      {
         kernel.Bind<IDataObjectRepository<AcceptanceCriteriaStatus>>().ToConstant( new DataObjectRepository<AcceptanceCriteriaStatus>() );
         kernel.Bind<IDataObjectRepository<ProjectStatus>>().ToConstant( new DataObjectRepository<ProjectStatus>() );
         kernel.Bind<IDataObjectRepository<SprintStatus>>().ToConstant( new DataObjectRepository<SprintStatus>() );
         kernel.Bind<IDataObjectRepository<WorkItemStatus>>().ToConstant( new DataObjectRepository<WorkItemStatus>() );
         kernel.Bind<IDataObjectRepository<WorkItemType>>().ToConstant( new DataObjectRepository<WorkItemType>() );
      }
   }
}
