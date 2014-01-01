using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScrum.Data.Domain;
using NHibernate;
using Ninject.Extensions.Logging;

namespace HomeScrum.Data.Services
{
   public class SprintCalendarService : ISprintCalendarService
   {
      private readonly ILogger _logger;
      private readonly ISessionFactory _sessionFactory;

      public SprintCalendarService(ILogger logger, ISessionFactory sessionFactory)
      {
         _logger = logger;
         _sessionFactory = sessionFactory;
      }

      public void Update( Sprint sprint )
      {
         Log.Debug( "Updating Sprint Calendar" );

         if (sprint.StartDate == null)
         {
            return;
         }

         Log.Debug( "Sprint Calendar Update Complete" );
      }

      protected ILogger Log
      {
         get { return _logger; }
      }
   }
}
