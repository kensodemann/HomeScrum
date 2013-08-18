[assembly: WebActivator.PreApplicationStartMethod( typeof( HomeScrum.Web.NinjectWebCommon ), "Start" )]
[assembly: WebActivator.ApplicationShutdownMethodAttribute( typeof( HomeScrum.Web.NinjectWebCommon ), "Stop" )]

namespace HomeScrum.Web
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
   using HomeScrum.Web.Models.Sprints;

   public static class NinjectWebCommon
   {
      private static readonly Bootstrapper bootstrapper = new Bootstrapper();

      public static IKernel Kernel { get { return bootstrapper.Kernel; } }

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
         kernel.Bind<ISecurityService>().To( typeof( SecurityService ) ).InSingletonScope();

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
         kernel.Bind<IPropertyNameTranslator<Sprint, SprintEditorViewModel>>()
            .ToConstant( new PropertyNameTranslator<Sprint, SprintEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel>>().ToConstant( new WorkItemPropertyNameTranslator() );
         kernel.Bind<IPropertyNameTranslator<User, UserEditorViewModel>>().ToConstant( new PropertyNameTranslator<User, UserEditorViewModel>() );

         kernel.Bind<IWebSecurity>().ToConstant( new WebSecurityWrapper() );

         // Only needed if actually doing injections from NHibernate...
         //kernel.Bind<NHibernate.Proxy.IProxyFactory>().To( typeof( NHibernate.Proxy.DefaultProxyFactory ) ).InSingletonScope();

         Mapper.Initialize( map => map.ConstructServicesUsing( x => kernel.Get( x ) ) );
      }
   }
}
