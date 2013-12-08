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
      public IEnumerable<HomeScrum.Spa.Models.WorkItemStatus> Get()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session.Query<HomeScrum.Data.Domain.WorkItemStatus>()
            .Select( x => new HomeScrum.Spa.Models.WorkItemStatus()
            {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description
            } )
            .ToList();
      }

      // GET api/<controller>/5
      public HomeScrum.Spa.Models.WorkItemStatus Get( string id )
      {
         Guid itemId;

         if (Guid.TryParse( id, out itemId ))
         {
            return FetchWorkItemStatus( itemId );
         }

         throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
      }

      private Models.WorkItemStatus FetchWorkItemStatus( Guid id )
      {
         var session = _sessionFactory.GetCurrentSession();
         var item = session.Get<HomeScrum.Data.Domain.WorkItemStatus>( id );

         if (item == null)
         {
            throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
         }

         return new HomeScrum.Spa.Models.WorkItemStatus()
         {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description
         };
      }

      // POST api/<controller>
      public void Post( [FromBody]HomeScrum.Spa.Models.WorkItemStatus value )
      {
         throw new NotImplementedException();
      }

      // PUT api/<controller>/5
      public void Put( string id, [FromBody]HomeScrum.Spa.Models.WorkItemStatus value )
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