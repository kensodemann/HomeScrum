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
   using HomeScrum.Data.Validators;

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
         RegisterValidators( kernel );
      }

      private static void RegisterRepositories( IKernel kernel )
      {
         kernel.Bind<IRepository<AcceptanceCriteriaStatus, Guid>>().ToConstant( new Repository<AcceptanceCriteriaStatus, Guid>() );
         kernel.Bind<IRepository<ProjectStatus, Guid>>().ToConstant( new Repository<ProjectStatus, Guid>() );
         kernel.Bind<IRepository<SprintStatus, Guid>>().ToConstant( new Repository<SprintStatus, Guid>() );
         kernel.Bind<IRepository<WorkItemStatus, Guid>>().ToConstant( new Repository<WorkItemStatus, Guid>() );
         kernel.Bind<IRepository<WorkItemType, Guid>>().ToConstant( new Repository<WorkItemType, Guid>() );
         kernel.Bind<IRepository<User, String>>().ToConstant( new Repository<User, String>() );

         kernel.Bind<ISecurityRepository>().ToConstant( new SecurityRepository() );
      }

      private static void RegisterValidators( IKernel kernel )
      {
         kernel.Bind<IValidator<AcceptanceCriteriaStatus>>().To( typeof( AcceptanceCriteriaStatusValidator ) );
         kernel.Bind<IValidator<ProjectStatus>>().To( typeof( ProjectStatusValidator ) );
         kernel.Bind<IValidator<SprintStatus>>().To( typeof( SprintStatusValidator ) );
         kernel.Bind<IValidator<WorkItemStatus>>().To( typeof( WorkItemStatusValidator ) );
         kernel.Bind<IValidator<WorkItemType>>().To( typeof( WorkItemTypeValidator ) );
      }
   }
}
