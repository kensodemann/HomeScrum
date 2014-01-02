﻿using HomeScrum.Data.Domain;
using NHibernate;
using Ninject;
using Ninject.Extensions.Logging;

namespace HomeScrum.Spa.Controllers
{
   public class ProjectsController : DomainObjectApiController<Project>
   {
      [Inject]
      public ProjectsController( ILogger logger, ISessionFactory sessionFactory )
         : base( logger, sessionFactory ) { }
   }
}