[assembly: WebActivator.PreApplicationStartMethod( typeof( HomeScrum.Web.App_Start.NinjectWebCommon ), "Start" )]
[assembly: WebActivator.ApplicationShutdownMethodAttribute( typeof( HomeScrum.Web.App_Start.NinjectWebCommon ), "Stop" )]

namespace HomeScrum.Web.App_Start
{
   using System;
   using System.Web;
   using AutoMapper;
   using HomeScrum.Common.Utility;
   using HomeScrum.Data.Domain;
   using HomeScrum.Data.Repositories;
   using HomeScrum.Data.Services;
   using HomeScrum.Web.Models.Admin;
   using HomeScrum.Web.Models.WorkItems;
   using HomeScrum.Web.Providers;
   using HomeScrum.Web.Translators;
   using Microsoft.Web.Infrastructure.DynamicModuleHelper;
   using NHibernate;
   using Ninject;
   using Ninject.Web.Common;

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
         RegisterTranslators( kernel );
         RegisterProviders( kernel );

         Mapper.Initialize( map => map.ConstructServicesUsing( x => kernel.Get( x ) ) );
      }

      private static void RegisterRepositories( IKernel kernel )
      {
         kernel.Bind<ISecurityService>().To( typeof( SecurityService ) ).InSingletonScope();

         kernel.Bind<ISessionFactory>().ToConstant( NHibernateHelper.SessionFactory );
      }

      private static void RegisterTranslators( IKernel kernel )
      {
         kernel.Bind<IPropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>>()
            .ToConstant( new PropertyNameTranslator<AcceptanceCriterionStatus, AcceptanceCriterionStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>>()
            .ToConstant( new PropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel>>()
            .ToConstant( new PropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>>()
            .ToConstant( new PropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel>>()
            .ToConstant( new PropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel>() );

         kernel.Bind<IPropertyNameTranslator<Project, ProjectEditorViewModel>>().ToConstant( new ProjectPropertyNameTranslator() );
         kernel.Bind<IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel>>().ToConstant( new WorkItemPropertyNameTranslator() );

         kernel.Bind<IPropertyNameTranslator<User, UserEditorViewModel>>().ToConstant( new PropertyNameTranslator<User, UserEditorViewModel>() );
      }

      private static void RegisterProviders( IKernel kernel )
      {
         kernel.Bind<IWebSecurity>().ToConstant( new WebSecurityWrapper() );
      }
   }
}
