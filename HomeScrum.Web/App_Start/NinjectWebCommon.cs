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
   using HomeScrum.Web.Providers;
   using AutoMapper;
   using HomeScrum.Web.Translators;
   using HomeScrum.Web.Models.Admin;
   using HomeScrum.Web.Models.WorkItems;

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
         RegisterTranslators( kernel );
         RegisterProviders( kernel );

         Mapper.Initialize( map => map.ConstructServicesUsing( x => kernel.Get( x ) ) );
      }

      private static void RegisterRepositories( IKernel kernel )
      {
         BindRepository<AcceptanceCriteriaStatus>( kernel );
         BindRepository<ProjectStatus>( kernel );
         BindRepository<SprintStatus>( kernel );
         BindRepository<WorkItemStatus>( kernel );
         BindRepository<WorkItemType>( kernel );

         BindRepository<Project>( kernel );
         BindRepository<WorkItem>( kernel );

         kernel.Bind<IUserRepository>().ToConstant( new UserRepository() );
         kernel.Bind<ISecurityRepository>().ToConstant( new SecurityRepository() );
      }

      private static void BindRepository<T>( IKernel kernel )
      {
         kernel.Bind<IRepository<T>>().ToConstant( new Repository<T>() );
      }

      private static void RegisterValidators( IKernel kernel )
      {
         kernel.Bind<IValidator<AcceptanceCriteriaStatus>>().To( typeof( AcceptanceCriteriaStatusValidator ) );
         kernel.Bind<IValidator<ProjectStatus>>().To( typeof( ProjectStatusValidator ) );
         kernel.Bind<IValidator<SprintStatus>>().To( typeof( SprintStatusValidator ) );
         kernel.Bind<IValidator<WorkItemStatus>>().To( typeof( WorkItemStatusValidator ) );
         kernel.Bind<IValidator<WorkItemType>>().To( typeof( WorkItemTypeValidator ) );

         kernel.Bind<IValidator<Project>>().To( typeof( NullValidator<Project> ) );
         kernel.Bind<IValidator<WorkItem>>().To( typeof( NullValidator<WorkItem> ) );

         kernel.Bind<IValidator<User>>().To( typeof( UserValidator ) );
      }

      private static void RegisterTranslators( IKernel kernel )
      {
         kernel.Bind<IPropertyNameTranslator<AcceptanceCriteria, AcceptanceCriteriaStatusEditorViewModel>>()
            .ToConstant( new GenericTranslator<AcceptanceCriteria, AcceptanceCriteriaStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<ProjectStatus, ProjectStatusEditorViewModel>>()
            .ToConstant( new GenericTranslator<ProjectStatus, ProjectStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<SprintStatus, SprintStatusEditorViewModel>>()
            .ToConstant( new GenericTranslator<SprintStatus, SprintStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>>()
            .ToConstant( new GenericTranslator<WorkItemStatus, WorkItemStatusEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<WorkItemType, WorkItemTypeEditorViewModel>>()
            .ToConstant( new GenericTranslator<WorkItemType, WorkItemTypeEditorViewModel>() );

         kernel.Bind<IPropertyNameTranslator<Project, ProjectEditorViewModel>>()
            .ToConstant( new GenericTranslator<Project, ProjectEditorViewModel>() );
         kernel.Bind<IPropertyNameTranslator<WorkItem, WorkItemEditorViewModel>>().ToConstant( new WorkItemEditorViewModelPropertyNameTranslator() );

         kernel.Bind<IPropertyNameTranslator<User, UserEditorViewModel>>().ToConstant( new GenericTranslator<User, UserEditorViewModel>() );
      }

      private static void RegisterProviders( IKernel kernel )
      {
         kernel.Bind<IWebSecurity>().ToConstant( new WebSecurityWrapper() );
      }
   }
}
