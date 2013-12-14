using HomeScrum.Data.Domain;
using NHibernate;
using NHibernate.Linq;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HomeScrum.Spa.Controllers
{
   public class WorkItemTypesController : ApiController
   {
      private readonly ISessionFactory _sessionFactory;
      private readonly ILogger _logger;

      protected ILogger Log { get { return _logger; } }

      [Inject]
      public WorkItemTypesController( ILogger logger, ISessionFactory sessionFactory )
      {
         _sessionFactory = sessionFactory;
         _logger = logger;
      }

      // GET api/<controller>
      public IEnumerable<WorkItemType> Get()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session.Query<WorkItemType>()
            .ToList();
      }

      // GET api/<controller>/5
      public WorkItemType Get( string id )
      {
         Guid witId;

         if (Guid.TryParse( id, out witId ))
         {
            return FetchWorkItemType( witId );
         }

         throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
      }

      private WorkItemType FetchWorkItemType( Guid witId )
      {
         var session = _sessionFactory.GetCurrentSession();
         var wit = session.Get<HomeScrum.Data.Domain.WorkItemType>( witId );

         if (wit == null)
         {
            throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
         }

         return wit;
      }

      // POST api/<controller>
      public void Post( [FromBody]WorkItemType value )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               session.Save( value );
               transaction.Commit();
            }
            catch (Exception e)
            {
               Log.Error( e, "Insert Failed" );
               transaction.Rollback();
               throw e;
            }
         }
      }

      // PUT api/<controller>/5
      public void Put( string id, [FromBody]WorkItemType value )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               session.Update( value );
               transaction.Commit();
            }
            catch (Exception e)
            {
               Log.Error( e, "Update Failed" );
               transaction.Rollback();
               throw e;
            }
         }
      }

      // DELETE api/<controller>/5
      public void Delete( int id )
      {
         throw new NotImplementedException( "Deletes are not allowed in this system" );
      }
   }
}