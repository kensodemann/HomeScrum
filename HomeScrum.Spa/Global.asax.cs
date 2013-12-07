using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using HomeScrum.Spa.App_Start;
using NHibernate;
using NHibernate.Context;
using Ninject;

namespace HomeScrum.Spa
{
   // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
   // visit http://go.microsoft.com/?LinkId=9394801
   public class MvcApplication : System.Web.HttpApplication
   {
      protected void Application_Start()
      {
         AreaRegistration.RegisterAllAreas();

         WebApiConfig.Register( GlobalConfiguration.Configuration );
         FilterConfig.RegisterGlobalFilters( GlobalFilters.Filters );
         RouteConfig.RegisterRoutes( RouteTable.Routes );

         NHibernateConfig.Configure();

         log4net.Config.XmlConfigurator.Configure();

         //ModelBinders.Binders[typeof( IPrincipal )] = new PrincipalModelBinder();
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