using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHibernate;
using NHibernate.Linq;
using Ninject;
using Ninject.Extensions.Logging;

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
      public IEnumerable<HomeScrum.Spa.Models.WorkItemType> Get()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session.Query<HomeScrum.Data.Domain.WorkItemType>()
            .Select( x => new HomeScrum.Spa.Models.WorkItemType()
            {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description
            } )
            .ToList();
      }

      // GET api/<controller>/5
      public HomeScrum.Spa.Models.WorkItemType Get( string id )
      {
         var session = _sessionFactory.GetCurrentSession();
         var witId = new Guid( id );
         var wit = session.Get<HomeScrum.Data.Domain.WorkItemType>( witId );

         if (wit == null)
         {
            throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
         }

         return new HomeScrum.Spa.Models.WorkItemType()
         {
            Id = wit.Id,
            Name = wit.Name,
            Description = wit.Description
         };
      }

      // POST api/<controller>
      public void Post( [FromBody]HomeScrum.Spa.Models.WorkItemType value )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               // Map to a domain type
               // session.Save(wit);
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
      public void Put( string id, [FromBody]HomeScrum.Spa.Models.WorkItemType value )
      {
         var session = _sessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            try
            {
               // Get the domain type
               // Update the data
               // session.Update(wit);
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