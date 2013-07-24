using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HomeScrum.Data.Domain;
using HomeScrum.Web.Extensions;
using HomeScrum.Web.Models.Base;
using HomeScrum.Web.Translators;
using NHibernate;
using Ninject.Extensions.Logging;

namespace HomeScrum.Web.Controllers.Base
{
   public class SystemDataObjectController<ModelT, ViewModelT, EditorViewModelT> : ReadWriteController<ModelT, ViewModelT, EditorViewModelT>
      where ModelT : SystemDomainObject
      where ViewModelT : SystemDomainObjectViewModel
      where EditorViewModelT : SystemDomainObjectViewModel, new()
   {
      public SystemDataObjectController( IPropertyNameTranslator<ModelT, EditorViewModelT> translator, ILogger logger, ISessionFactory sessionFactory )
         : base( translator, logger, sessionFactory ) { }


      //
      // POST: /ModelTs/UpdateSortOrders
      [HttpPost]
      public ActionResult UpdateSortOrders( IEnumerable<string> itemIds )
      {
         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var idIndex = 0;
            foreach (var id in itemIds)
            {
               var item = session.Get<ModelT>( new Guid( id ) );
               idIndex++;
               if (item != null && item.SortSequence != idIndex)
               {
                  item.SortSequence = idIndex;
                  session.Update( item );
               }
            }
            transaction.Commit();
         }

         return new EmptyResult();
      }

      //
      // GET: /ModelT/
      public override ActionResult Index()
      {
         Log.Debug( "Index()" );

         var session = SessionFactory.GetCurrentSession();
         using (var transaction = session.BeginTransaction())
         {
            var query = new HomeScrum.Data.Queries.AllSystemObjectsOrdered<ModelT>();
            var items = query.GetQuery( session )
               .SelectSystemDomainObjectViewModels<ModelT>();

            transaction.Commit();
            return View( items );
         }

      }
   }
}