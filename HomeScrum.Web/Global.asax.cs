using HomeScrum.Web.Binders;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using NHibernate;
using NHibernate.Context;

namespace HomeScrum.Web
{
   // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
   // visit http://go.microsoft.com/?LinkId=9394801

   public class MvcApplication : System.Web.HttpApplication
   {
      protected void Application_Start()
      {
         AreaRegistration.RegisterAllAreas();

         GlobalConfiguration.Configure( WebApiConfig.Register );
         FilterConfig.RegisterGlobalFilters( GlobalFilters.Filters );
         RouteConfig.RegisterRoutes( RouteTable.Routes );
         BundleConfig.RegisterBundles( BundleTable.Bundles );
         AuthConfig.RegisterAuth();
         MapperConfig.RegisterMappings();

         NHibernateConfig.Configure();

         log4net.Config.XmlConfigurator.Configure();

         ModelBinders.Binders[typeof( IPrincipal )] = new PrincipalModelBinder();
      }

      protected void Application_BeginRequest()
      {
         var sessionFactory = NinjectWebCommon.Kernel.Get<ISessionFactory>();
         var session = sessionFactory.OpenSession();
         CurrentSessionContext.Bind( session );
      }

      protected void Application_EndRequest()
      {
         var sessionFactory = NinjectWebCommon.Kernel.Get<ISessionFactory>();
         var session = CurrentSessionContext.Unbind( sessionFactory );
         session.Dispose();
      }
   }
}