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
   public class WorkItemStatusesController : ApiController
   {
      private readonly ISessionFactory _sessionFactory;
      private readonly ILogger _logger;

      protected ILogger Log { get { return _logger; } }

      [Inject]
      public WorkItemStatusesController( ILogger logger, ISessionFactory sessionFactory )
      {
         _sessionFactory = sessionFactory;
         _logger = logger;
      }


      // GET api/<controller>
      public IEnumerable<WorkItemStatus> Get()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session.Query<WorkItemStatus>()
            .ToList();
      }

      // GET api/<controller>/5
      public WorkItemStatus Get( string id )
      {
         Guid itemId;

         if (Guid.TryParse( id, out itemId ))
         {
            return FetchWorkItemStatus( itemId );
         }

         throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
      }

      private WorkItemStatus FetchWorkItemStatus( Guid id )
      {
         var session = _sessionFactory.GetCurrentSession();
         var item = session.Get<WorkItemStatus>( id );

         if (item == null)
         {
            throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
         }

         return item;
      }

      // POST api/<controller>
      public void Post( [FromBody]WorkItemStatus value )
      {
         throw new NotImplementedException();
      }

      // PUT api/<controller>/5
      public void Put( string id, [FromBody]WorkItemStatus value )
      {
         throw new NotImplementedException();
      }

      // DELETE api/<controller>/5
      public void Delete( int id )
      {
         throw new NotImplementedException( "Deletes are not allowed in this system" );
      }
   }
}