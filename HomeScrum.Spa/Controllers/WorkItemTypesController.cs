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
      private ISessionFactory _sessionFactory;
      private ILogger _logger;

      [Inject]
      public WorkItemTypesController( ILogger logger, ISessionFactory sessionFactory )
      {
         _sessionFactory = sessionFactory;
         _logger = logger;
      }

      // GET api/<controller>
      public IEnumerable<HomeScrum.Spa.Models.WorkItemType> Get()
      {
         using (var session = _sessionFactory.GetCurrentSession())
         {
            return session.Query<HomeScrum.Data.Domain.WorkItemType>()
               .Select( x=> new HomeScrum.Spa.Models.WorkItemType()
               {
                  Id = x.Id,
                  Name = x.Name,
                  Description = x.Description
               } )
               .ToList();
         }
      }

      // GET api/<controller>/5
      public HomeScrum.Spa.Models.WorkItemType Get( string id )
      {
         using (var session = _sessionFactory.GetCurrentSession())
         {
            var witId = new Guid( id );
            var wit = session.Get<HomeScrum.Data.Domain.WorkItemType>(witId);
            return new HomeScrum.Spa.Models.WorkItemType()
            {
               Id = wit.Id,
               Name = wit.Name,
               Description = wit.Description
            };
         }
      }

      // POST api/<controller>
      public void Post( [FromBody]string value )
      {
      }

      // PUT api/<controller>/5
      public void Put( int id, [FromBody]string value )
      {
      }

      // DELETE api/<controller>/5
      public void Delete( int id )
      {
      }
   }
}