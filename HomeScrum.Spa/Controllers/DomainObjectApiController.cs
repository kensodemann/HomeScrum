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
   [Authorize]
   public class DomainObjectApiController<T> : ApiController
      where T: DomainObjectBase
   {
      private readonly ISessionFactory _sessionFactory;
      private readonly ILogger _logger;

      protected ILogger Log { get { return _logger; } }

      [Inject]
      public DomainObjectApiController( ILogger logger, ISessionFactory sessionFactory )
      {
         _sessionFactory = sessionFactory;
         _logger = logger;
      }

      // GET api/<controller>
      public IEnumerable<T> Get()
      {
         var session = _sessionFactory.GetCurrentSession();

         return session.Query<T>()
            .ToList();
      }

      // GET api/<controller>/5
      public T Get( string id )
      {
         Guid witId;

         if (Guid.TryParse( id, out witId ))
         {
            return FetchItem( witId );
         }

         throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
      }

      private T FetchItem( Guid itemId )
      {
         var session = _sessionFactory.GetCurrentSession();
         var item = session.Get<T>( itemId );

         if (item == null)
         {
            throw new HttpResponseException( new HttpResponseMessage( HttpStatusCode.NotFound ) );
         }

         return item;
      }

      // POST api/<controller>
      public void Post( [FromBody]T value )
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
      public void Put( string id, [FromBody]T value )
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